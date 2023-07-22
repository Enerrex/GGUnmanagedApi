using UnmanagedAPI.Containers;

namespace UnmanagedCore.Containers.Hashing.HashKeyHandlers
{
    public struct IntKeyHandler : IHashProvider<int>
    {
        public int GetHash(in int item)
        {
            return item.GetHashCode();
        }
    }
}