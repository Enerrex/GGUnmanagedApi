using System;

namespace UnmanagedAPI.Containers
{
    public interface IHashProvider<TUnmanaged> : IEquatable<TUnmanaged> where TUnmanaged : unmanaged
    {
        public int GetHashCode(in TUnmanaged item);
    }
}