using System;

namespace GGUnmanagedApi.Core.Containers
{
    public unsafe struct LinkedNode<T> : IDisposable where T : unmanaged
    {
        public T Value;
        public Allocation.Owner<LinkedNode<T>> Next;
        
        public LinkedNode
        (
            T value
        )
        {
            Value = value;
            Next = default;
        }

        public static implicit operator T
        (
            LinkedNode<T> node
        )
        {
            return node.Value;
        }

        public void Dispose()
        {
            if (Next == IntPtr.Zero) return;
            var next = Next;
            next.Dispose();
        }
    }
}