using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnmanagedAPI;
using UnmanagedAPI.Containers;
using UnmanagedAPI.Extensions;
using UnmanagedCore.Containers.Iterators;

namespace UnmanagedCore.Containers.Hashing
{
    // HashSet uses Hopscotch hashing
    public unsafe struct HashSet<TUnmanagedKey, TUnmanagedKeyHandler>
        where TUnmanagedKey : unmanaged, IEquatable<TUnmanagedKey> 
    {
        private Allocation.Owner<LinkedList<Entry>> _storage;

        public int Count { get; private set; }
        public int Capacity { get; private set; }
        
        private Configuration _configuration;

        public HashSet
        (
            int capacity,
            Configuration? configuration=null
        )
        {
            _storage = Allocation.Initialize<LinkedList<Entry>>
            (
                capacity
            );
            Capacity = capacity;
            _configuration = configuration ?? Configuration.Default;
            Count = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int GetBucketIndex
        (
            TUnmanagedKey key
        )
        {
            int hash = key.GetHashCode();
            return WrapIndex(hash == int.MaxValue ? 0 : hash);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe LinkedList<Entry>* GetHomeBucketPointer
        (
            TUnmanagedKey key
        )
        {
            var index = GetBucketIndex(key);
            return GetIndexPointer(index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe LinkedList<Entry>* GetIndexPointer
        (
            int index
        )
        {
            return _storage.ToPointer(index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int WrapIndex
        (
            int index
        )
        {
            return index % Capacity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool AreKeysEqual
        (
            TUnmanagedKey keyA,
            TUnmanagedKey keyB
        )
        {
            // return _keyHandler.AreEqual(keyA, keyB);
            return keyA.Equals(keyB);
        }

        public void Add
        (
            TUnmanagedKey key
        )
        {
            // if (Contains(key)) return;
            // if (LoadFactor > 0.75f) Resize(Capacity * 2);
            Insert(key);
            Count++;
        }

        private unsafe void Insert
        (
            TUnmanagedKey key
        )
        {
            int bucket_ix = GetBucketIndex(key);
            var entry = new Entry
            {
                Hash = bucket_ix,
                Key = key
            };
            var home_bucket = GetIndexPointer(bucket_ix);
            if (home_bucket->IsEmpty)
            {
                *home_bucket = new LinkedList<Entry>(entry);
                return;
            }
            
            if (home_bucket->Count < _configuration.MaximumBucketSize)
            {
                home_bucket->AddNode(entry);
                return;
            }

            // Resize
            Resize();
        }

        /// <summary>
        /// Removes a key from the list.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>True if a key is removed.</returns>
        public bool Remove
        (
            TUnmanagedKey key
        )
        {
            int bucket_ix = GetBucketIndex(key);
            var home_bucket = GetIndexPointer(bucket_ix);
            if (home_bucket->IsEmpty) return false;

            var iterator = Iterator.Get(*home_bucket);
            var (success, index) = iterator.MoveNext();
            while (success)
            {
                if (AreKeysEqual(key, iterator.Current.Key))
                {
                    home_bucket->RemoveAt(index);
                    Count -= 1;
                    return true;
                }
                (success, index) = iterator.MoveNext();
            }
            return false;
        }

        public bool HasKey
        (
            TUnmanagedKey key
        )
        {
            int bucket_ix = GetBucketIndex(key);
            var home_bucket = GetIndexPointer(bucket_ix);
            if (home_bucket->IsEmpty) return false;

            var iterator = Iterator.Get(*home_bucket);
            while (iterator.MoveNext().Success)
            {
                var entry = iterator.Current;
                if (AreKeysEqual(key, entry.Key)) return true;
            }

            return false;
        }

        internal void Resize()
        {
            
            var new_capacity = Capacity;
            if (_configuration.ExpansionStrategy == Configuration.ExpansionStrategyEnum.Scalar)
            {
                new_capacity *= _configuration.ExpansionValue;
            }
            else
            {
                new_capacity += _configuration.ExpansionValue;
            }
            Capacity = new_capacity;
            
            var old_storage = _storage;
            _storage = Allocation.Initialize<LinkedList<Entry>>
            (
                Capacity
            );
            
            for (int bucket_ix = 0; bucket_ix < old_storage.Pointer->Count; bucket_ix++)
            {
                var bucket = GetIndexPointer(bucket_ix);
                if (bucket->IsEmpty) continue;
                var iterator = Iterator.Get(*bucket);
                while (iterator.MoveNext().Success)
                {
                    var entry = iterator.Current;
                    Insert(entry.Key);
                }
            }
            
            old_storage.DisposeWithUnderlying();
        }

        public void Dispose()
        {
            // for (int bucket_ix = 0; bucket_ix < Capacity; bucket_ix++)
            // {
            //     _storage[bucket_ix].Dispose();
            // }

            _storage.Dispose();
        }

        internal struct Entry
        {
            public TUnmanagedKey Key;
            public int Hash;
        }

        public struct Configuration
        {
            public static Configuration Default => new Configuration
            {
                MaximumBucketSize = 8,
                ExpansionStrategy = ExpansionStrategyEnum.Scalar,
                ExpansionValue = 2
            };
            
            public enum ExpansionStrategyEnum
            {
                Scalar,
                Linear
            }
            
            public int MaximumBucketSize;
            public ExpansionStrategyEnum ExpansionStrategy;
            public int ExpansionValue;
        }
    }
}