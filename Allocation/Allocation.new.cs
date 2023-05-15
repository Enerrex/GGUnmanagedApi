using API.Pointer;

namespace API.Allocation
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
                (TUnmanaged*) Malloc<TUnmanaged>
                (
                    length
                )
            );
        }
        
        public static AllocationOwner<TUnmanaged> Create<TUnmanaged>
        (
            TUnmanaged value
        ) where TUnmanaged : unmanaged
        {
            var owner = new AllocationOwner<TUnmanaged>
            (
                (TUnmanaged*) Malloc<TUnmanaged>
                (
                    1
                )
            );
            owner[0] = value;
            return owner;
        }
    }
}