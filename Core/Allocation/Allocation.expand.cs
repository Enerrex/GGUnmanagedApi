using GGUnmanagedApi.Core.Pointer;

namespace GGUnmanagedApi.Core
{
    public static unsafe partial class Allocation
    {
        public static AllocationOwner<TUnmanaged> Expand<TUnmanaged>(in AllocationReference<TUnmanaged> copiedArray, int length, int targetLength) where TUnmanaged : unmanaged
        {
            return Copy(copiedArray, length, targetLength);
        }
    }
}