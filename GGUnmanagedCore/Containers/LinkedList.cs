using System;
using UnmanagedAPI;
using UnmanagedAPI.Iterator;
using UnmanagedCore.Containers.Iterators;

namespace UnmanagedCore.Containers
{
    public unsafe struct LinkedList<TUnmanaged> :
        IDisposable
        where TUnmanaged : unmanaged
    {
        public int Count { get; private set; }
        public bool IsEmpty => Count == 0;

        // Allocation owner for the Head node.
        private Allocation.Owner<LinkedNode<TUnmanaged>> _head;
        public readonly Allocation.Reference<LinkedNode<TUnmanaged>> Head => _head.ToReference();
        public Allocation.Reference<LinkedNode<TUnmanaged>> Tail { get; private set; }

        public LinkedList
        (
            TUnmanaged valueIn
        ) : this()
        {
            AddNode(valueIn);
        }

        public void AddNode
        (
            TUnmanaged valueIn
        )
        {
            Count += 1;

            // If the list is empty, initialize it by creating a new node.
            if (Head.IsNull)
            {
                // This list becomes the owner of the Head allocation.
                // Each subsequent node will be owned by the previous node
                var allocation_owner = Allocation.Initialize
                (
                    new LinkedNode<TUnmanaged>(valueIn)
                );
                // Set the Head Owner
                _head = allocation_owner;
                Tail = allocation_owner.ToReference();
                return;
            }

            // Head is not null, pass the value to the tail, it will create a new node.
            Tail = Tail.Pointer->SetNext(valueIn);
        }

        public void AddNode
        (
            LinkedList<TUnmanaged> valueIn
        )
        {
            var other_list_node = valueIn.Head;
            while (other_list_node.Pointer != null)
            {
                AddNode(other_list_node.Pointer->Value);
                other_list_node = other_list_node.Pointer->Next;
            }
        }

        public void AddNode
        (
            in LinkedNode<TUnmanaged> node
        )
        {
            throw new NotImplementedException();
        }

        // Insert at an index, shifting the node at the index to the right
        public void InsertAt
        (
            int index,
            TUnmanaged valueIn
        )
        {
            if (index < 0 || index >= Count) throw new IndexOutOfRangeException();

            // If the index is 0, set the Head to the new node
            if (index == 0)
            {
                // We are transferring ownership of _head from this list to the new node
                var old_head = _head;
                _head = Allocation.Initialize
                (
                    new LinkedNode<TUnmanaged>(valueIn)
                );
                _head.Pointer->SetNext(old_head.ToReference());
            }
            else
            {
                // Otherwise, find the node at the index and insert the new node after it
                var node = Head;
                for (int i = 0; i < index; i++)
                {
                    node = node.Pointer->Next;
                }

                // Section: Repurpose the target node to house the added valueIn
                //  Create a new node and set the target node's next to point to it
                //  Fill the new node's value with the old value of the target node
                //  Point the new node at the old next node

                // start by capturing the old value and next node
                var old_value = node.Pointer->Value;
                var old_next = node.Pointer->Next;

                // Set the target node's value to the new value
                node.Pointer->Value = valueIn;

                // Create a new node, fill it with the target node's old value
                var new_node = Allocation.Initialize
                (
                    new LinkedNode<TUnmanaged>(old_value)
                );

                // Set the target node's next to the new node
                node.Pointer->SetNext(new_node.ToReference());
                // Set the new node's next to the old next node
                new_node.Pointer->SetNext(old_next);
                // End Section
            }
        }

        public void RemoveAt
        (
            int index
        )
        {
            if (index < 0 || index >= Count) throw new IndexOutOfRangeException();
            
            // If the index is 0, set the Head to the new node
            if (index == 0)
            {
                // We are transferring ownership of _head from this list to the new node
                var old_head = _head;
                _head = new Allocation.Owner<LinkedNode<TUnmanaged>>(old_head.Pointer->Next);
                old_head.Pointer->ClearNext();
                old_head.Dispose();
            }
            else
            {
                // Otherwise, find the node at the index and insert the new node after it
                var node = Head;
                var node_prev = Head;
                for (int i = 0; i < index; i++)
                {
                    node_prev = node;
                    node = node.Pointer->Next;
                }

                if (node.IsNull) throw new NullReferenceException();
                node_prev.Pointer->SetNext(node.Pointer->Next);
                node.Pointer->ClearNext();
                node.Pointer->Dispose();
            }
        }

        public readonly LinkedList<TUnmanaged> GetCopy()
        {
            var new_list = new LinkedList<TUnmanaged>();
            var node = Head;
            while (node.Pointer != null)
            {
                new_list.AddNode(node.Pointer->Value);
                node = node.Pointer->Next;
            }

            return new_list;
        }

        public void Dispose()
        {
            if (Head.Pointer != null) (~Head)->Dispose();

            _head.Dispose();
            _head = Allocation.Owner<LinkedNode<TUnmanaged>>.Null;
        }
    }
}