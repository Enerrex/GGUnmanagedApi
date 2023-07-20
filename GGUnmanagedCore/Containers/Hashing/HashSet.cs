using System;
using UnmanagedAPI.Containers;

namespace UnmanagedCore.Containers.Hashing
{
    public struct HashSet<TUnmanagedKey, TUnmanagedKeyHandler>
        where TUnmanagedKey : unmanaged, IEquatable<TUnmanagedKey>
        where TUnmanagedKeyHandler : unmanaged, IHashProvider<TUnmanagedKey>
    {
        private TUnmanagedKeyHandler _keyHandler;

        private PointerList<PointerList<TUnmanagedKey>> _buckets;

        public int Count { get; private set; }

        public HashSet(int bucketCount)
        {
            _keyHandler = new TUnmanagedKeyHandler();

            _buckets = new PointerList<PointerList<TUnmanagedKey>>(bucketCount);
            for (int bucket_ix = 0; bucket_ix < bucketCount; bucket_ix++)
            {
                _buckets[bucket_ix] = new PointerList<TUnmanagedKey>(1);
            }

            Count = 0;
        }

        public void Add(TUnmanagedKey item)
        {
            int bucket_ix = GetBucketIndex(item);
            var bucket = _buckets[bucket_ix];
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
        
        internal int GetBucketIndex(TUnmanagedKey item)
        {
            int hash = _keyHandler.GetHash(item);
            int bucket_ix = hash % _buckets.Count;
            return bucket_ix;
        }
        
        internal int GetIndexInBucket(TUnmanagedKey item)
        {
            var bucket = _buckets[GetBucketIndex(item)];
            for (int item_ix = 0; item_ix < bucket.Count; item_ix++)
            {
                if (bucket[item_ix].Equals(item))
                {
                    return item_ix;
                }
            }

            return -1;
        }

        public bool Remove(TUnmanagedKey item)
        {
            int hash = _keyHandler.GetHash(item);
            int bucket_ix = hash % _buckets.Count;
            var bucket = _buckets[bucket_ix];
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
            int bucket_ix = hash % _buckets.Count;
            var bucket = _buckets[bucket_ix];
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
            for (int bucket_ix = 0; bucket_ix < _buckets.Count; bucket_ix++)
            {
                _buckets[bucket_ix].Dispose();
                _buckets[bucket_ix] = new PointerList<TUnmanagedKey>(1);
            }
            
            Count = 0;
        }
        
        public void Dispose()
        {
            for (int bucket_ix = 0; bucket_ix < _buckets.Count; bucket_ix++)
            {
                _buckets[bucket_ix].Dispose();
            }
            
            _buckets.Dispose();
        }
    }
}