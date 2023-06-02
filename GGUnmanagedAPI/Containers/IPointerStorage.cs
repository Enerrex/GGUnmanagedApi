namespace UnmanagedAPI.Containers
{
    public interface IPointerStorage<TUnmanaged> where TUnmanaged : unmanaged
    {
        public Allocation.Reference<TUnmanaged> Reference { get; }
        
        public Allocation.Slice<TUnmanaged> GetSlice
        (
            int start = 0,
            int? length = null
        );
    }
}