namespace UnmanagedAPI.Containers
{
    public interface IHashProvider<TUnmanaged> where TUnmanaged : unmanaged
    {
        public int GetHash(in TUnmanaged item);
    }
}