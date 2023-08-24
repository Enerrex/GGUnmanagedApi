using System;

namespace UnmanagedAPI.Containers
{
    public interface IHashProvider<TUnmanaged> where TUnmanaged : unmanaged
    {
        public int GetHashCode(in TUnmanaged item);
    }
}