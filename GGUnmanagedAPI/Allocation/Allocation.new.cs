using UnmanagedAPI.Pointer;

namespace UnmanagedAPI
{
    public static unsafe partial class Allocation
    {
        public static AllocationOwner<TUnmanaged> Create<TUnmanaged>
        (
            int length = 1
        ) where TUnmanaged : unmanaged
        {
            return new AllocationOwner<TUnmanaged>
            (
                (TUnmanaged*)Malloc<TUnmanaged>
                (
                    length
                ).Pointer
            );
        }

        public static AllocationOwner CreateNonGeneric<TUnmanaged>
        (
            int length
        ) where TUnmanaged : unmanaged
        {
            return Malloc<TUnmanaged>
            (
                length
            );
        }
        
        public static AllocationOwner CreateNonGeneric<TUnmanaged>
        (
            TUnmanaged value
        ) where TUnmanaged : unmanaged
        {
            var owner = Malloc<TUnmanaged>
            (
                1
            );
            *owner.As<TUnmanaged>() = value;
            return owner;
        }
    }
}