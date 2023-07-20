using UnmanagedAPI.Containers;

namespace UnmanagedCore.Containers.Hashing
{
    public struct HashMapIntKey<TUnmanagedValue> where TUnmanagedValue : unmanaged
    {
        internal struct IntHasher : IHashProvider<int>
        {
            public int GetHash(in int item)
            {
                return item;
            }
        }
        
        private HashMap<int, IntHasher, TUnmanagedValue> _proxiedMap;
        
        public int Count => _proxiedMap.Count;
        
        public HashMapIntKey(int capacity)
        {
            _proxiedMap = new HashMap<int, IntHasher, TUnmanagedValue>(capacity);
        }
        
        public void Add(int key, TUnmanagedValue value)
        {
            _proxiedMap.Add(key, value);
        }
        
        public bool Remove(int key)
        {
            return _proxiedMap.Remove(key);
        }
        
        
    }
}