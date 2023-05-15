using System;
using UnmanagedAPI;
using UnmanagedAPI.Pointer;

namespace Core.Containers
{
    /// <summary>
    ///     Important usage note: this struct circumvents the AllocationOwner struct because of a CLR limitation
    ///     We can't have a struct that as a generic type that references itself (even though it's a pointer)
    ///     We can however have a struct that has a raw pointer to itself.
    /// </summary>
    /// <typeparam name="TUnmanaged"></typeparam>
    public unsafe struct LinkedNode<TUnmanaged> : IOwner<LinkedNode<TUnmanaged>>, IDisposable
        where TUnmanaged : unmanaged
    {
        public readonly TUnmanaged Value;

        public AllocationReference<LinkedNode<TUnmanaged>> Next =>
            new AllocationReference<LinkedNode<TUnmanaged>>(_next);

        private LinkedNode<TUnmanaged>* _next;

        public LinkedNode
        (
            TUnmanaged value
        )
        {
            Value = value;
            _next = default;
        }

        public void SetNext
        (
            TUnmanaged value
        )
        {
            if (_next != null) _next->Dispose();
            // This is bad! We're throwing away the AllocationOwner object because of a CLR limitation
            var allocation = Allocation.Create
            (
                new LinkedNode<TUnmanaged>(value)
            );
            _next = allocation.Pointer;
        }

        // Potentially unsafe, previous node must be disposed of manually
        public void SetNext
        (
            LinkedNode<TUnmanaged> value
        )
        {
            *_next = value;
        }

        public static implicit operator TUnmanaged
        (
            LinkedNode<TUnmanaged> node
        )
        {
            return node.Value;
        }

        public AllocationReference<LinkedNode<TUnmanaged>> ToReference()
        {
            return new AllocationReference<LinkedNode<TUnmanaged>>(_next);
        }

        public void Dispose()
        {
            if ((IntPtr)Next.Pointer == IntPtr.Zero) return;
            // These must happen in this order
            //     Dispose of the next LinkedNode (which will dispose of the next LinkedNode, etc.)
            _next->Dispose();
            //     Dispose of our own allocation for the next node
            //     This is bad! We shouldn't ever do this anywhere else
            Allocation.Delete(this);
        }
    }
}