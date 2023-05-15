using API.Pointer;

namespace API.Allocation
{
    public static unsafe partial class Allocation
    {
        public static AllocationOwner<TUnmanaged> Initialize<TUnmanaged>
        (
            TUnmanaged value,
            int length
        ) where TUnmanaged : unmanaged
        {
            var allocated = Create<TUnmanaged>(length);

            for (var i = 0; i < length; i++) *(allocated.Pointer + i) = value;
            return allocated;
        }
        
        public static AllocationOwner<TUnmanaged> Initialize<TUnmanaged>
        (
            int length
        ) where TUnmanaged : unmanaged
        {
            var allocated = Create<TUnmanaged>(length);

            for (var i = 0; i < length; i++) *(allocated.Pointer + i) = new TUnmanaged();
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