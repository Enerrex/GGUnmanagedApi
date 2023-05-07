using GGUnmanagedApi.Core.Pointer;

namespace GGUnmanagedApi.Core
{
    public static unsafe partial class Allocation
    {
        public static void Delete<T>
        (
            AllocationOwner<T> allocationOwner
        ) where T : unmanaged
        {
            Free(allocationOwner);
        }
    }
}