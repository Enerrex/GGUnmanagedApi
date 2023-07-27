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
        internal static readonly int LookAheadLimit = 256;

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
            return WrapIndex(hash == int.MaxValue ? 0 : hash);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int WrapIndex(int index)
        {
            return index % Capacity;
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

            // Find next open slot
            var distance = 1;
            var current_ix = (bucket_ix + distance) % Capacity;
            HomeBucket* current_bucket = GetIndexPointer(current_ix);

            while (distance < LookAheadLimit)
            {
                if (!current_bucket->IsOccupied)
                {
                    break;
                }

                distance++;
                current_ix = (bucket_ix + distance) % Capacity;
                current_bucket = GetIndexPointer(current_ix);
            }

            if (distance < LookAheadLimit)
            {
                // Empty slot is IN the neighborhood
                if (distance < NeighborHoodSize)
                {
                    home_bucket->SetBit(distance);
                    current_bucket->Key = key;
                    return;
                }
            }

        }
        
        internal unsafe void FindCLoserFreeBucket(HomeBucket* bucket, ref int distance)
        {
            HomeBucket* target_bucket = bucket - NeighborHoodSize - 1;

            while (distance < LookAheadLimit)
            {
                if (!current_bucket->IsOccupied)
                {
                    break;
                }

                distance++;
                current_ix = (bucket->Hash + distance) % Capacity;
                current_bucket = GetIndexPointer(current_ix);
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

            public void SetBit(int bitIndex)
            {
                NeighborHood |= 1 << bitIndex;
            }

            public bool GetBit(int bitIndex)
            {
                return (NeighborHood & (1 << bitIndex)) != 0;
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