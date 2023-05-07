using System;
using GGUnmanagedApi.Core.Pointer;

namespace GGUnmanagedApi.Core.Containers
{
    /// <summary>
    /// WARN: This struct has a functionality to expand infinitely as long as elements are added.
    /// Every time capacity is reached, the array is reallocated at ~twice the previous size.
    /// </summary>
    /// <typeparam name="TUnmanaged"></typeparam>
    public unsafe struct PointerList<TUnmanaged> : IDisposable where TUnmanaged : unmanaged
    {
        private AllocationOwner<TUnmanaged> AllocationOwner { get; set; }

        public int Capacity { get; private set; }
        public int Count { get; private set; }

        public PointerList
        (
            int capacity
        ) : this()
        {
            if (capacity <= 0) throw new ArgumentException();
            Capacity = capacity;
            Count = 0;
            AllocationOwner = Allocation.AllocateNew<TUnmanaged>(capacity);
        }

        public PointerList
        (
            TUnmanaged valueIn
        ) : this(capacity: 1)
        {
            AllocationOwner.Pointer[0] = valueIn;
        }

        public PointerList
        (
            PointerList<TUnmanaged> copiedList
        ) : this(capacity: copiedList.Capacity)
        {
            Count = copiedList.Count;
            AllocationOwner = Allocation.Copy
            (
                copiedList.AllocationOwner.ToReference(),
                Capacity,
                Capacity
            );
        }

        private readonly bool CheckIndexOutOfRange
        (
            int index
        )
        {
            return (index < 0 || index >= Capacity);
        }

        public TUnmanaged this
        [
            int index
        ]
        {
            get
            {
                if (CheckIndexOutOfRange(index)) throw new IndexOutOfRangeException();
                return *(AllocationOwner.Pointer + index);
            }
        }

        public TUnmanaged Get
        (
            int index
        )
        {
            if (CheckIndexOutOfRange(index)) throw new IndexOutOfRangeException();
            return *(AllocationOwner.Pointer + index);
        }

        public readonly TUnmanaged* GetPointer
        (
            int index
        )
        {
            if (CheckIndexOutOfRange(index)) throw new IndexOutOfRangeException();
            return AllocationOwner.Pointer + index;
        }

        /// <summary>
        /// Adds a new element. Expands the array if Count exceeds to Length
        /// </summary>
        /// <param name="value"></param>
        public void Add
        (
            TUnmanaged value
        )
        {
            if (CheckIndexOutOfRange(Count))
            {
                // Copy the old array to a new array with double the capacity
                var new_storage = Allocation.Expand
                (
                    AllocationOwner.ToReference(),
                    Capacity,
                    // Double capacity
                    Capacity * 2
                );
                AllocationOwner.Dispose();
                AllocationOwner = new_storage;
                Capacity *= 2;
            }
            *(AllocationOwner.Pointer + Count) = value;
            Count++;
        }

        public void Add
        (
            in PointerList<TUnmanaged> values
        )
        {
            var new_count = Count + values.Count;

            // Determine new count, then expand if necessary
            if (CheckIndexOutOfRange(Count))
            {
                var new_capacity = Capacity * 2 + values.Count;
                // Increase capacity by the Count of the added values + the standard 2x
                var new_storage = Allocation.Expand
                (
                    AllocationOwner.ToReference(),
                    Capacity,
                    new_capacity
                );
                AllocationOwner.Dispose();
                AllocationOwner = new_storage;
                Capacity = new_capacity;
            }

            // Copy values from the source array to the target (this) array
            AllocationReference<TUnmanaged> starting_pointer = AllocationOwner.Pointer + Count;
            Allocation.CopyTo
            (
                sourceAllocation: values.AllocationOwner.ToReference(),
                sourceLength: values.Count,
                targetAllocation: starting_pointer,
                targetLength: values.Count
            );
            Count = new_count;
        }

        public void Dispose()
        {
            if (Capacity == 0) return;
            Capacity = 0;
            AllocationOwner.Dispose();
        }
    }
}