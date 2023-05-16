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

        public static PointerOwner CreatePointerOwner<TUnmanaged>
        (
            int length
        ) where TUnmanaged : unmanaged
        {
            return new PointerOwner
            (
                Malloc<TUnmanaged>
                (
                    length
                )
            );
        }
    }
}