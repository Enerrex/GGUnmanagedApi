namespace UnmanagedAPI
{
    public static unsafe partial class Allocation
    {
        public static Owner<TUnmanaged> Create<TUnmanaged>
        (
            int length = 1
        ) where TUnmanaged : unmanaged
        {
            return new Owner<TUnmanaged>
            (
                (TUnmanaged*)Malloc<TUnmanaged>
                (
                    length
                ).IntPtr
            );
        }

        public static Owner CreateNonGeneric<TUnmanaged>
        (
            int length
        ) where TUnmanaged : unmanaged
        {
            return Malloc<TUnmanaged>
            (
                length
            );
        }
        
        public static Owner CreateNonGeneric<TUnmanaged>
        (
            TUnmanaged value
        ) where TUnmanaged : unmanaged
        {
            var owner = Malloc<TUnmanaged>
            (
                1
            );
            *owner.As<TUnmanaged>() = value;
            return owner;
        }
    }
}