using System;

namespace UnmanagedAPI.Pointer
{
    public readonly unsafe struct AllocationOwner
    {
        public IntPtr Pointer { get; }
        
        public bool IsNull => Pointer == IntPtr.Zero;

        public AllocationOwner
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
        
        public TUnmanaged* As<TUnmanaged>() where TUnmanaged : unmanaged
        {
            return (TUnmanaged*)Pointer;
        }

        public void Dispose()
        {
            Allocation.Delete(this);
        }
    }
}