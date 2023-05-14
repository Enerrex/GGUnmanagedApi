using Core.Pointer;

namespace Core
{
    public static unsafe partial class Allocation
    {
        public static AllocationOwner<TUnmanaged> Initialize<TUnmanaged>
        (
            TUnmanaged? value = null,
            int length = 1
        ) where TUnmanaged : unmanaged
        {
            var allocated = AllocateNew<TUnmanaged>(length);

            for (var i = 0; i < length; i++) *(allocated.Pointer + i) = value ?? new TUnmanaged();
            return allocated;
        }

        public static void Initialize<TUnmanaged>
        (
            AllocationReference<TUnmanaged> allocationReference,
            int allocationLength
        ) where TUnmanaged : unmanaged
        {
            for (var i = 0; i < allocationLength; i++) *(allocationReference.ToPointer() + i) = new TUnmanaged();
        }
    }
}