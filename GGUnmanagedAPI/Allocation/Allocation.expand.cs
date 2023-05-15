using UnmanagedAPI.Pointer;

namespace UnmanagedAPI
{
    public static partial class Allocation
    {
        public static AllocationOwner<TUnmanaged> CopyToNew<TUnmanaged>
        (
            in AllocationReference<TUnmanaged> copiedArray,
            int length,
            int targetLength
        ) where TUnmanaged : unmanaged
        {
            return Copy
            (
                copiedArray,
                length,
                targetLength
            );
        }
    }
}