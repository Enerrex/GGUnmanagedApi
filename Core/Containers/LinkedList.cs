using System;
using Core.Pointer;

namespace Core.Containers
{
    public unsafe struct LinkedList<TUnmanaged> : IDisposable where TUnmanaged : unmanaged
    {
        private AllocationOwner<LinkedNode<TUnmanaged>> _head;
        public int Count { get; private set; }
        public readonly AllocationReference<LinkedNode<TUnmanaged>> Head
        {
            get => _head.ToReference();
        }
        public AllocationReference<LinkedNode<TUnmanaged>> Tail { get; private set; }

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
            var allocation_owner = Allocation.Initialize<LinkedNode<TUnmanaged>>
            (
                new LinkedNode<TUnmanaged>(valueIn)
            );
            if (Head.Pointer == null)
            {
                _head = allocation_owner;
                Tail = allocation_owner.ToReference();
                return;
            }

            Tail.Pointer->SetNext(*allocation_owner.Pointer);
            Tail = allocation_owner.ToReference();
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
            if (Head.Pointer != null) Head.Pointer->Dispose();

            _head.Dispose();
        }
    }
}