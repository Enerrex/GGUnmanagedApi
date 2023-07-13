namespace UnmanagedAPI.Containers
{
    public interface ISliceable<TUnmanaged> where TUnmanaged : unmanaged
    {
        public Allocation.Slice<TUnmanaged> GetSlice
        (
            int start = 0,
            int? length = null
        );
    }
}