﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnmanagedAPI;
using UnmanagedAPI.Containers;
using UnmanagedAPI.Iterator;
using UnmanagedCore.Containers.Iterators;
using UnmanagedCore.Debug.Proxies;

namespace UnmanagedCore.Containers
{
    /// <summary>
    ///     WARN: This struct has a functionality to expand infinitely as long as elements are added.
    ///     Every time length is reached, the array is reallocated at ~twice the previous size.
    /// </summary>
    /// <typeparam name="TUnmanaged"></typeparam>
    [DebuggerTypeProxy(typeof(PointerListProxy<>))]
    public unsafe struct PointerList<TUnmanaged> :
        IPointerStorage<TUnmanaged>,
        IDisposable
        where TUnmanaged : unmanaged
    {
        private Allocation.Owner<TUnmanaged> _owner;
        public Allocation.Reference<TUnmanaged> Reference => _owner.ToReference();

        public int Capacity { get; private set; }
        public int Count { get; private set; }

        public PointerList
        (
            int capacity
        ) : this()
        {
            if (capacity <= 0) throw new ArgumentException();
            Capacity = capacity;
            _owner = Allocation.Create<TUnmanaged>(capacity);
        }

        public PointerList
        (
            TUnmanaged valueIn
        ) : this(1)
        {
            // Will add the value and increment count for us
            Add(valueIn);
        }

        public PointerList
        (
            PointerList<TUnmanaged> copiedList
        ) : this(copiedList.Capacity)
        {
            Allocation.CopyTo
            (
                copiedList._owner.ToReference(),
                copiedList.Capacity,
                _owner.ToReference(),
                copiedList.Capacity
            );
            Count = copiedList.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly bool CheckCapacityExceeded
        (
            int index
        )
        {
            return index >= Capacity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly bool CheckIndexOutOfRange
        (
            int index
        )
        {
            return index < 0 || index >= Capacity;
        }

        public TUnmanaged this
        [
            int index
        ]
        {
            get
            {
                if (CheckIndexOutOfRange(index)) throw new IndexOutOfRangeException();
                return *_owner.ToPointer(index);
            }
            set
            {
                if (CheckIndexOutOfRange(index)) throw new IndexOutOfRangeException();
                *_owner.ToPointer(index) = value;
            }
        }
        
        public Allocation.Slice<TUnmanaged> GetSlice
        (
            int startIndex = 0,
            int? length = null
        )
        {
            int length_value = length ?? Count - startIndex;
            if (_owner.IsNull) throw new NullReferenceException();
            if (CheckIndexOutOfRange(startIndex)) throw new IndexOutOfRangeException();
            if (CheckIndexOutOfRange(startIndex + length_value - 1)) throw new IndexOutOfRangeException();
            return new Allocation.Slice<TUnmanaged>
            (
                Reference,
                startIndex,
                length_value
            );
        }

        public TUnmanaged Get
        (
            int index
        )
        {
            return *_owner.ToPointer(index);
        }
        

        /// <summary>
        ///     Adds a new element. Expands the array if Count exceeds to Length
        /// </summary>
        /// <param name="value"></param>
        public void Add
        (
            TUnmanaged value
        )
        {
            // Double the length if the count exceeds the length
            if (CheckCapacityExceeded(Count))
            {
                ExpandCapacity(Capacity * 2);
            }
            
            *_owner.ToPointer(Count) = value;
            Count++;
        }

        public void Add
        (
            in PointerList<TUnmanaged> values
        )
        {
            var updated_count = Count + values.Count;
            // Determine new count, then expand if necessary
            // Subtract 1 from the updated count because we don't want to expand if the count is exactly equal to the capacity
            if (CheckCapacityExceeded(updated_count - 1))
            {
                // Update the length to be double the current length plus the number of elements to be added
                // E.g. if current length is 10 and we want to add 11 elements, the new length will be 10 * 2 + 11 = 31
                // whereas if we only doubled the length, we would have 10 * 2 = 20
                var updated_capacity = Capacity * 2 + values.Count;
                ExpandCapacity(updated_capacity);
            }

            // Copy values from the source array to the target (this) array
            Allocation.Reference<TUnmanaged> starting_pointer = _owner.Pointer + Count;
            Allocation.CopyTo
            (
                values._owner.ToReference(),
                values.Count,
                starting_pointer,
                values.Count
            );
            Count += values.Count;
        }

        // This does not guarantee that the memory is zeroed out
        // Accessing the memory after this operation may yield the old element
        public void RemoveAt(int index)
        {
            if (CheckIndexOutOfRange(index)) throw new IndexOutOfRangeException();
            if (index == Count - 1)
            {
                Count--;
                return;
            }
            // Copy the elements after the index to the index
            Allocation.CopyTo<TUnmanaged>
            (
                _owner.ToPointer(index + 1),
                Count - 1,
                _owner.ToPointer(index),
                Count - 1
            );
            Count--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExpandCapacity
        (
            int updatedCapacity
        )
        {
            // Copy the old array to a new array with the specified length
            var new_storage = Allocation.CopyToNew
            (
                _owner.ToReference(),
                Capacity,
                updatedCapacity
            );
            Capacity = updatedCapacity;
            _owner.Dispose();
            _owner = new_storage;
        }

        public readonly PointerList<TUnmanaged> GetCopy()
        {
            return new PointerList<TUnmanaged>(this);
        }

        public void Dispose()
        {
            if (Capacity == 0) return;
            _owner.Dispose();
            _owner = Allocation.Owner<TUnmanaged>.Null;
            Capacity = 0;
            Count = 0;
        }
    }
}