using GGUnmanagedApi.Core.Pointer;

namespace GGUnmanagedApi.Core
{
    public static unsafe partial class Allocation
    {
        public static AllocationOwner<TUnmanaged> Initialize<TUnmanaged>
        (
            int length = 1,
            TUnmanaged? value = null
        ) where TUnmanaged : unmanaged
        {
            var allocated = new AllocationOwner<TUnmanaged>
            (
                (TUnmanaged*) Malloc<TUnmanaged>
                (
                    SizeOf<TUnmanaged>() * length
                )
            );
            
            for (var i = 0; i < length; i++)
            {
                *(allocated.Pointer + i) = value ?? new TUnmanaged();
            }
            return allocated;
        }
        
        public static void Initialize<TUnmanaged>
        (
           AllocationReference<TUnmanaged> allocationReference,
           int allocationLength
        ) where TUnmanaged : unmanaged
        {
            for (var i = 0; i < allocationLength; i++)
            {
                *(allocationReference.ToPointer() + i) = new TUnmanaged();
            }
        }
    }
}