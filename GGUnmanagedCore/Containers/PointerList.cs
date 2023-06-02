using System;
using System.Runtime.CompilerServices;
using UnmanagedAPI;

namespace UnmanagedCore.Containers
{
    /// <summary>
    ///     WARN: This struct has a functionality to expand infinitely as long as elements are added.
    ///     Every time length is reached, the array is reallocated at ~twice the previous size.
    /// </summary>
    /// <typeparam name="TUnmanaged"></typeparam>
    public unsafe struct PointerList<TUnmanaged> : IDisposable where TUnmanaged : unmanaged
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
            _owner[0] = valueIn;
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
                return _owner[index];
            }
            set
            {
                if (CheckIndexOutOfRange(index)) throw new IndexOutOfRangeException();
                _owner[index] = value;
            }
        }

        public TUnmanaged Get
        (
            int index
        )
        {
            return this[index];
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
            _owner[Count] = value;
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

        public void Dispose()
        {
            if (Capacity == 0) return;
            _owner.Dispose();
        }
    }
}