using System;
using UnmanagedAPI;
using UnmanagedAPI.Pointer;

namespace Core.Containers
{
    public unsafe struct LinkedList<TUnmanaged> : IDisposable where TUnmanaged : unmanaged
    {
        public int Count { get; private set; }
        public bool IsEmpty => Count == 0;

        private AllocationOwner<LinkedNode<TUnmanaged>> _head;
        public readonly AllocationReference<LinkedNode<TUnmanaged>> Head => _head.ToReference();
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

            if (Head.Pointer == null)
            {
                var allocation_owner = Allocation.Initialize
                (
                    new LinkedNode<TUnmanaged>(valueIn)
                );
                _head = allocation_owner;
                Tail = allocation_owner.ToReference();
                return;
            }

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