namespace UnmanagedAPI.Containers
{
    public interface IPointerStorage<TUnmanaged> : ISliceable<TUnmanaged> where TUnmanaged : unmanaged
    {
        public Allocation.Reference<TUnmanaged> Reference { get; }
    }
}