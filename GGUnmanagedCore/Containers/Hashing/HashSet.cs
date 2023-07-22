using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnmanagedAPI.Containers;

namespace UnmanagedCore.Containers.Hashing
{
    public struct HashSet<TUnmanagedKey, TUnmanagedKeyHandler>
        where TUnmanagedKey : unmanaged, IEquatable<TUnmanagedKey>
        where TUnmanagedKeyHandler : unmanaged, IHashProvider<TUnmanagedKey>
    {
        private TUnmanagedKeyHandler _keyHandler;

        private PointerList<TUnmanagedKey> _storage;

        public int Count { get; private set; }

        public HashSet(int capacity)
        {
            var a = true;
            var b = a!;
            _keyHandler = new TUnmanagedKeyHandler();

            _storage = new PointerList<TUnmanagedKey>(capacity);
            for (int bucket_ix = 0; bucket_ix < capacity; bucket_ix++)
            {
                _storage[bucket_ix] = new PointerList<TUnmanagedKey>(1);
            }

            Count = 0;
        }

        public void Add(TUnmanagedKey item)
        {
            int bucket_ix = GetBucketIndex(item);
            var bucket = _storage[bucket_ix];
            for (int item_ix = 0; item_ix < bucket.Count; item_ix++)
            {
                if (bucket[item_ix].Equals(item))
                {
                    return;
                }
            }

            bucket.Add(item);
            Count++;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal uint GetBucketIndex(TUnmanagedKey key)
        {
            uint hash = (uint)key.GetHashCode();
            return hash == uint.MaxValue ? 0 : hash;
        }
        
        public bool Remove(TUnmanagedKey item)
        {
            int hash = _keyHandler.GetHash(item);
            int bucket_ix = hash % _storage.Count;
            var bucket = _storage[bucket_ix];
            for (int item_ix = 0; item_ix < bucket.Count; item_ix++)
            {
                if (bucket[item_ix].Equals(item))
                {
                    bucket.RemoveAt(item_ix);
                    Count--;
                    return true;
                }
            }

            return false;
        }
        
        public bool Contains(TUnmanagedKey item)
        {
            int hash = _keyHandler.GetHash(item);
            int bucket_ix = hash % _storage.Count;
            var bucket = _storage[bucket_ix];
            for (int item_ix = 0; item_ix < bucket.Count; item_ix++)
            {
                if (bucket[item_ix].Equals(item))
                {
                    return true;
                }
            }

            return false;
        }
        
        public void Clear()
        {
            for (int bucket_ix = 0; bucket_ix < _storage.Count; bucket_ix++)
            {
                _storage[bucket_ix].Dispose();
                _storage[bucket_ix] = new PointerList<TUnmanagedKey>(1);
            }
            
            Count = 0;
        }
        
        public void Dispose()
        {
            for (int bucket_ix = 0; bucket_ix < _storage.Count; bucket_ix++)
            {
                _storage[bucket_ix].Dispose();
            }
            
            _storage.Dispose();
        }
    }
}