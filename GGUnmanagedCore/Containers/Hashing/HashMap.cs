using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnmanagedAPI;
using UnmanagedAPI.Containers;
using UnmanagedCore.Containers.Iterators.Extensions;

namespace UnmanagedCore.Containers.Hashing
{
    public struct HashMap<TUnmanagedKey, TUnmanagedKeyHandler, TUnmanagedValue> 
        where TUnmanagedKey : unmanaged, IEquatable<TUnmanagedKey>
        where TUnmanagedKeyHandler : unmanaged, IHashProvider<TUnmanagedKey>
        where TUnmanagedValue : unmanaged
    {
        private TUnmanagedKeyHandler _keyHandler;
        public int Count { get; private set; }
        private PointerList<PointerList<TUnmanagedKey>> _bucketsKeys;
        private PointerList<PointerList<TUnmanagedValue>> _bucketsValues;

        public HashMap(int capacity)
        {
            _bucketsKeys = new PointerList<PointerList<TUnmanagedKey>>(capacity);
            _bucketsValues = new PointerList<PointerList<TUnmanagedValue>>(capacity);
            for (int bucket_ix = 0; bucket_ix < capacity; bucket_ix++)
            {
                _bucketsValues[bucket_ix] = new PointerList<TUnmanagedValue>(1);
            }

            Count = 0;
            _keyHandler = new TUnmanagedKeyHandler();
        }
        
        // Indexer
        public TUnmanagedValue this[TUnmanagedKey key]
        {
            get
            {
                if (TryGetValue(key, out var value))
                {
                    return value;
                }

                throw new KeyNotFoundException("Key does not exist in HashMap");
            }
            set
            {
                if (!ContainsKey(key)) throw new KeyNotFoundException();
                Set(key, value);
            }
        }
        
                
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int GetBucketIndex(TUnmanagedKey item)
        {
            int hash = _keyHandler.GetHashCode(item);
            int bucket_ix = hash % _bucketsKeys.Count;
            return bucket_ix;
        }
        
        public void Add(TUnmanagedKey item, TUnmanagedValue value)
        {
            var bucket_ix = GetBucketIndex(item);
            var key_bucket = _bucketsKeys[bucket_ix];

            for (int item_ix = 0; item_ix < key_bucket.Count; item_ix++)
            {
                if (key_bucket[item_ix].Equals(item))
                {
                    throw new InvalidOperationException("Key already exists in HashMap");
                }
            }

            var value_bucket = _bucketsValues[bucket_ix];
            key_bucket.Add(item);
            value_bucket.Add(value);
            Count++;
        }
        
        private void Set(TUnmanagedKey item, TUnmanagedValue value)
        {
            int hash = _keyHandler.GetHashCode(item);
            int bucket_ix = hash % _bucketsKeys.Count;

            var key_bucket = _bucketsKeys[bucket_ix];
            var value_bucket = _bucketsValues[bucket_ix];
            for (int item_ix = 0; item_ix < key_bucket.Count; item_ix++)
            {
                if (key_bucket[item_ix].Equals(item))
                {
                    value_bucket[item_ix] = value;
                    return;
                }
            }

            throw new KeyNotFoundException();
        }
        
        public bool TryAdd(TUnmanagedKey item, TUnmanagedValue value)
        {
            int hash = _keyHandler.GetHashCode(item);
            int bucket_ix = hash % _bucketsKeys.Count;

            var key_bucket = _bucketsKeys[bucket_ix];
            var value_bucket = _bucketsValues[bucket_ix];
            for (int item_ix = 0; item_ix < key_bucket.Count; item_ix++)
            {
                if (key_bucket[item_ix].Equals(item))
                {
                    return false;
                }
            }

            key_bucket.Add(item);
            value_bucket.Add(value);
            Count++;
            return true;
        }
        
        public bool Remove(TUnmanagedKey item)
        {
            int hash = _keyHandler.GetHashCode(item);
            int bucket_ix = hash % _bucketsKeys.Count;
            var key_bucket = _bucketsKeys[bucket_ix];
            var value_bucket = _bucketsValues[bucket_ix];
            for (int item_ix = 0; item_ix < key_bucket.Count; item_ix++)
            {
                if (key_bucket[item_ix].Equals(item))
                {
                    key_bucket.RemoveAt(item_ix);
                    value_bucket.RemoveAt(item_ix);
                    Count--;
                    return true;
                }
            }
            
            return false;
        }
        
        public bool TryGetValue(TUnmanagedKey item, out TUnmanagedValue result)
        {
            result = default;
            int hash = _keyHandler.GetHashCode(item);
            int bucket_ix = hash % _bucketsKeys.Count;
            var key_bucket = _bucketsKeys[bucket_ix];
            var value_bucket = _bucketsValues[bucket_ix];
            for (int item_ix = 0; item_ix < key_bucket.Count; item_ix++)
            {
                if (key_bucket[item_ix].Equals(item))
                {
                    result = value_bucket[item_ix];
                    return true;
                }
            }
            
            return false;
        }

        public void ResizeBuckets(int size)
        {
            var new_keys = new PointerList<PointerList<TUnmanagedKey>>(size);
            var new_values = new PointerList<PointerList<TUnmanagedValue>>(size);
            var old_keys = _bucketsKeys;
            var old_values = _bucketsValues;
            _bucketsKeys = new_keys;
            _bucketsValues = new_values;
            
            // For each bucket, get each item and rehash it
            for (int bucket_ix = 0; bucket_ix < old_keys.Count; bucket_ix++)
            {
                var key_bucket = old_keys[bucket_ix];
                var value_bucket = old_values[bucket_ix];
                for (int item_ix = 0; item_ix < key_bucket.Count; item_ix++)
                {
                    var key = key_bucket[item_ix];
                    Add(key, value_bucket[item_ix]);
                }
                key_bucket.Dispose();
                value_bucket.Dispose();
            }
            
            old_keys.Dispose();
            old_values.Dispose();
        }
        
        public bool ContainsKey(TUnmanagedKey item)
        {
            int hash = _keyHandler.GetHashCode(item);
            int bucket_ix = hash % _bucketsKeys.Count;
            var key_bucket = _bucketsKeys[bucket_ix];
            for (int item_ix = 0; item_ix < key_bucket.Count; item_ix++)
            {
                if (key_bucket[item_ix].Equals(item))
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}