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
        internal float LoadFactor => Count / (float) Capacity;

        private Allocation.Owner<HomeBucket> _storage;

        public int Count { get; private set; }
        public int Capacity { get; private set; }

        internal static readonly int NeighborHoodSize = 32;
        internal static readonly int LookAheadLimit = 256;

        public HashSet
        (
            int capacity
        )
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
        internal int GetBucketIndex
        (
            TUnmanagedKey key
        )
        {
            int hash = _keyHandler.GetHashCode(key);
            return WrapIndex(hash == int.MaxValue ? 0 : hash);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe HomeBucket* GetHomeBucketPointer
        (
            TUnmanagedKey key
        )
        {
            var index = GetBucketIndex(key);
            return GetIndexPointer(index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe HomeBucket* GetIndexPointer
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

        private unsafe void Insert
        (
            TUnmanagedKey key
        )
        {
            #region TryHomeBucket
            int bucket_ix = GetBucketIndex(key);
            var home_bucket = GetIndexPointer(bucket_ix);
            if (!home_bucket->IsOccupied)
            {
                home_bucket->Key = key;
                home_bucket->SetOccupied();
                return;
            }
            #endregion

            #region FindEmptySlot
            // Find next open slot
            var distance_to_empty_slot = 1;
            var current_ix = (bucket_ix + distance_to_empty_slot);
            HomeBucket* current_bucket = GetIndexPointer(current_ix);

            // Ensure we don't go out of bounds
            var look_ahead_limit = Math.Min(LookAheadLimit, Capacity);
            while (distance_to_empty_slot < look_ahead_limit)
            {
                if (!current_bucket->IsOccupied)
                {
                    break;
                }

                distance_to_empty_slot++;
                current_ix = (bucket_ix + distance_to_empty_slot);
                current_bucket = GetIndexPointer(current_ix);
            }
            #endregion

            if (distance_to_empty_slot < look_ahead_limit)
            {
                // Empty slot is IN the neighborhood
                if (distance_to_empty_slot < NeighborHoodSize)
                {
                    home_bucket->SetBit(distance_to_empty_slot);
                    current_bucket->Key = key;
                    return;
                }
                
                MoveBucketAway(ref current_bucket, ref distance_to_empty_slot);
            }

        }

        /// <summary>
        /// Attempts to move a bucket away from its home bucket (while still being in the neighborhood)
        /// This will be called multiple times by Insert until we reach a limit
        /// or the new key is added into its neighborhood
        /// </summary>
        /// <param name="targetBucket"></param>
        /// <param name="distance"></param>
        internal unsafe void MoveBucketAway
        (
            ref HomeBucket* targetBucket,
            ref int distance
        )
        {
            HomeBucket* target_bucket = targetBucket - NeighborHoodSize - 1;

            var capped_neighbor_hood_size = Math.Min(NeighborHoodSize, distance);
            for
            (
                var hop_distance_ix = NeighborHoodSize - 1;
                hop_distance_ix > 0;
                hop_distance_ix--
            )
            {
                // Try to find an open bucket in the neighborhood
                int open_bucket_ix = -1;
                // Loop through each bit up to the current hop distance
                for (var bit_ix = 0; bit_ix < hop_distance_ix; bit_ix++)
                {
                    if (target_bucket->GetBit(bit_ix))
                    {
                        open_bucket_ix = bit_ix;
                        break;
                    }
                }
                
                // There is an open bucket
                if (open_bucket_ix != -1)
                {
                    // Move the key to the open targetBucket
                    var open_bucket = target_bucket + open_bucket_ix;
                    open_bucket->Key = targetBucket->Key;
                    open_bucket->SetBit(hop_distance_ix);
                    targetBucket->Key = default;
                    targetBucket->SetBit(open_bucket_ix);
                    targetBucket = open_bucket;
                    distance -= hop_distance_ix;
                    return;
                }

                target_bucket++;
            }
            targetBucket = null;
            distance = 0;
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

            public void SetBit
            (
                int bitIndex
            )
            {
                NeighborHood |= 1 << bitIndex;
            }
            
            public void ClearBit
            (
                int bitIndex
            )
            {
                NeighborHood &= ~(1 << bitIndex);
            }

            public bool GetBit
            (
                int bitIndex
            )
            {
                return (NeighborHood & (1 << bitIndex)) != 0;
            }

            public static readonly HomeBucket Default = new HomeBucket
            {
                Key = default,
                // Default neighborhood is 0
                NeighborHood = 0
            };
            
            public void SetOccupied()
            {
                NeighborHood |= 1 << 0;
            }
        }
    }
}