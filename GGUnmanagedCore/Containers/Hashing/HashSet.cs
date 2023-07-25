using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnmanagedAPI;
using UnmanagedAPI.Containers;

namespace UnmanagedCore.Containers.Hashing
{
    // HashSet uses Hopscotch hashing
    public struct HashSet<TUnmanagedKey, TUnmanagedKeyHandler>
        where TUnmanagedKey : unmanaged, IEquatable<TUnmanagedKey>
        where TUnmanagedKeyHandler : unmanaged, IHashProvider<TUnmanagedKey>
    {
        private TUnmanagedKeyHandler _keyHandler;
        internal float LoadFactor => Count / (float)Capacity;
        
        private Allocation.Owner<HomeBucket> _storage;

        public int Count { get; private set; }
        public int Capacity { get; private set; }
        
        internal static readonly int NeighborHoodSize = 32;

        public HashSet(int capacity)
        {
            // TODO: Calc capacity from input
            _storage = Allocation.Initialize
            (
                HomeBucket.Default,
                capacity
            );
            Capacity = capacity;
            Count = 0;
            _keyHandler = new TUnmanagedKeyHandler();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int GetBucketIndex(TUnmanagedKey key)
        {
            int hash = _keyHandler.GetHashCode(key);
            return hash == int.MaxValue ? 0 : hash;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe HomeBucket* GetHomeBucketPointer(TUnmanagedKey key)
        {
            var index = GetBucketIndex(key);
            return GetIndexPointer(index);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe HomeBucket* GetIndexPointer(int index)
        {
            return _storage.ToPointer(index);
        }

        private unsafe void Insert(TUnmanagedKey key)
        {
            int bucket_ix = GetBucketIndex(key);
            var home_bucket = GetIndexPointer(bucket_ix);
            if (!home_bucket->IsOccupied)
            {
                home_bucket->Key = key;
                home_bucket->SetOccupied();
                return;
            }

            int neighborhood = home_bucket->NeighborHood;
            for (int bit_ix = 0; bit_ix < NeighborHoodSize; bit_ix++)
            {
                if (!home_bucket->GetBit(bit_ix))
                {
                    home_bucket->SetBit(bit_ix);
                    var bucket = GetIndexPointer(bucket_ix + bit_ix + 1);
                    bucket->Key = key;
                    bucket->NeighborHood = 0;
                    return;
                }
            }
        }

        public void Dispose()
        {
            for (int bucket_ix = 0; bucket_ix < Capacity; bucket_ix++)
            {
                _storage[bucket_ix].Dispose();
            }
            
            _storage.Dispose();
        }
        
        internal struct Entry
        {
            public TUnmanagedKey Key;
            public int Hash;
        }
        
        internal struct HomeBucket
        {
            public bool IsOccupied => (NeighborHood & (1 << 0)) != 0;
            public TUnmanagedKey Key;
            public int NeighborHood;
            
            public void SetBit(int bit_index)
            {
                NeighborHood |= 1 << bit_index;
            }
            
            public bool GetBit(int bit_index)
            {
                return (NeighborHood & (1 << bit_index)) != 0;
            }
            
            public static readonly HomeBucket Default = new HomeBucket
            {
                Key = default,
                // Default neighborhood is 0
                NeighborHood = 0
            };
            
            // Use the sign bit to indicate if the bucket is occupied
            public void SetOccupied()
            {
                NeighborHood |= 1 << 0;
            }
        }
    }
}