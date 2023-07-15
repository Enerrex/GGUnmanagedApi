namespace UnmanagedAPI.Containers
{
    public interface ISliceableRO<TUnmanaged> where TUnmanaged : unmanaged
    {
        public Allocation.SliceRO<TUnmanaged> GetSlice
        (
            int start = 0,
            int? length = null
        );
    }
}