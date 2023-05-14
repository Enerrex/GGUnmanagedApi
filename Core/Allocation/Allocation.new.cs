using Core.Pointer;

namespace Core
{
    public static unsafe partial class Allocation
    {
        public static AllocationOwner<TUnmanaged> AllocateNew<TUnmanaged>
        (
            int length = 1
        ) where TUnmanaged : unmanaged
        {
            return new AllocationOwner<TUnmanaged>
            (
                (TUnmanaged*) Malloc<TUnmanaged>
                (
                    length
                )
            );
        }
    }
}