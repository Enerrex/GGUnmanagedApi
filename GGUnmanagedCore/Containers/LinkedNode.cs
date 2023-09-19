using System;
using UnmanagedAPI;

namespace UnmanagedCore.Containers
{
    public unsafe struct LinkedNode<TUnmanaged> : IDisposable
        where TUnmanaged : unmanaged
    {
        public TUnmanaged Value;

        public Allocation.Reference<LinkedNode<TUnmanaged>>  Next => _next.ToReference<LinkedNode<TUnmanaged>>();

        private Allocation.Owner _next;

        public LinkedNode
        (
            TUnmanaged value
        )
        {
            Value = value;
            _next = default;
        }

        /// <summary>
        /// Adds a new LinkedNode to the end of the list with the specified value.
        /// Returns a reference to the new LinkedNode.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Allocation.Reference<LinkedNode<TUnmanaged>> SetNext
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
        
        // This MUST be used in the context of a transfer of ownership
        internal void SetNext
        (
            Allocation.Reference<LinkedNode<TUnmanaged>> next
        )
        {
            _next = new Allocation.Owner(next.IntPtr);
        }
        
        // Only used when replacing a node and we don't want to dispose of the next node
        internal void ClearNext()
        {
            _next = Allocation.Owner.Null;
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