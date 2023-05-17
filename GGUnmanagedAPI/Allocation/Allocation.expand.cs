using UnmanagedAPI.Pointer;

namespace UnmanagedAPI
{
    public static partial class Allocation
    {
        public static Owner<TUnmanaged> CopyToNew<TUnmanaged>
        (
            in Reference<TUnmanaged> copiedArray,
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