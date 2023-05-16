using System;

namespace UnmanagedAPI.Pointer
{
    public readonly unsafe struct PointerOwner
    {
        public IntPtr Pointer { get; }

        public PointerOwner
        (
            IntPtr pointer
        )
        {
            Pointer = pointer;
        }

        public AllocationReference<TUnmanaged> ToReference<TUnmanaged>() where TUnmanaged : unmanaged
        {
            return new AllocationReference<TUnmanaged>((TUnmanaged*)Pointer);
        }

        public void Dispose()
        {
            Allocation.Delete(this);
        }
    }
}