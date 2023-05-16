using System;
using UnmanagedAPI;
using UnmanagedAPI.Pointer;

namespace Core.Containers
{
    public unsafe struct LinkedNode<TUnmanaged> : IDisposable
        where TUnmanaged : unmanaged
    {
        public readonly TUnmanaged Value;

        public AllocationReference<LinkedNode<TUnmanaged>> Next => _next.ToReference<LinkedNode<TUnmanaged>>();

        private AllocationOwner _next;

        public LinkedNode
        (
            TUnmanaged value
        )
        {
            Value = value;
            _next = default;
        }

        public AllocationReference<LinkedNode<TUnmanaged>> SetNext
        (
            TUnmanaged value
        )
        {
            _next = Allocation.CreateNonGeneric
            (
                new LinkedNode<TUnmanaged>(value)
            );
            return _next.ToReference<LinkedNode<TUnmanaged>>();
        }
        
        public static implicit operator TUnmanaged
        (
            LinkedNode<TUnmanaged> node
        )
        {
            return node.Value;
        }
        
        public void Dispose()
        {
            if ((IntPtr) Next.Pointer == IntPtr.Zero) return;
            // These must happen in this order
            //     Dispose of the next LinkedNode (which will dispose of the next LinkedNode, etc.)
            _next.As<LinkedNode<TUnmanaged>>()->Dispose();
            //     Dispose of our own allocation for the next node
            _next.Dispose();
        }
    }
}