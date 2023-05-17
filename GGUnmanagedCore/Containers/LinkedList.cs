using System;
using UnmanagedAPI;

namespace Core.Containers
{
    public unsafe struct LinkedList<TUnmanaged> : IDisposable where TUnmanaged : unmanaged
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

        public void AddNode(in LinkedNode<TUnmanaged> node)
        {
            
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

        public readonly NodeIterator<TUnmanaged> GetIterator()
        {
            return new NodeIterator<TUnmanaged>(Head.Pointer);
        }

        public void Dispose()
        {
            if (Head.Pointer != null) (~Head)->Dispose();

            _head.Dispose();
        }
    }
}