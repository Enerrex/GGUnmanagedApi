using System;

namespace GGUnmanagedApi.Core.Containers
{
    // TODO: explore custom allocator to pre-allocate blocks of memory
    public unsafe struct LinkedList<T> : IDisposable where T : unmanaged
    {
        public int Count { get; private set; }
        public LinkedNode<T>* Head { get; private set; }
        public LinkedNode<T>* Tail { get; private set; }

        public LinkedList(T valueIn) : this()
        {
            AddNode(valueIn);
        }

        public void AddNode
        (
            T valueIn
        )
        {
            Count += 1;
            var pointer = Allocation.AllocateNew<LinkedNode<T>>();
            pointer.Pointer->Value = valueIn;
            if (Head == null)
            {
                Head = pointer;
                Tail = pointer;
                return;
            }

            Tail->Next = pointer;
            Tail = pointer;
        }

        public void AddNode
        (
            LinkedList<T> valueIn
        )
        {
            var temp = valueIn.Head;
            while (temp != null)
            {
                AddNode(temp->Value);
                temp = temp->Next;
            }
        }

        public LinkedList<T> GetCopy()
        {
            var new_list = new LinkedList<T>();
            var node = Head;
            while (node != null)
            {
                new_list.AddNode(node->Value);
                node = node->Next;
            }

            return new_list;
        }

        public NodeIterator<T> GetIterator()
        {
            return new NodeIterator<T>(Head);
        }

        public void Dispose()
        {
            if (Head != null) Head->Dispose();

            Head = null;
            Tail = null;
        }
        
        public override string ToString()
        {
            return $"({string.Join(",", GetNativeArray().ToArray())})";
        }
    }
}