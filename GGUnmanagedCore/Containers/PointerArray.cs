using System;
using UnmanagedAPI;
using UnmanagedAPI.Pointer;

namespace Core.Containers
{
    /// <summary>
    ///     WARN: This struct has a functionality to expand infinitely as long as elements are added.
    ///     Every time length is reached, the array is reallocated at ~twice the previous size.
    /// </summary>
    /// <typeparam name="TUnmanaged"></typeparam>
    public unsafe struct PointerArray<TUnmanaged> : IDisposable where TUnmanaged : unmanaged
    {
        private AllocationOwner<TUnmanaged> AllocationOwner { get; }
        public AllocationReference<TUnmanaged> AllocationReference => AllocationOwner.ToReference();

        public int Length { get; private set; }

        public PointerArray
        (
            int length
        ) : this()
        {
            if (length <= 0) throw new ArgumentException();
            Length = length;
            AllocationOwner = Allocation.Create<TUnmanaged>(length);
        }

        public PointerArray
        (
            TUnmanaged valueIn
        ) : this(1)
        {
            AllocationOwner.Pointer[0] = valueIn;
        }

        public PointerArray
        (
            PointerArray<TUnmanaged> copiedList
        ) : this(copiedList.Length)
        {
            AllocationOwner = Allocation.Copy
            (
                copiedList.AllocationOwner.ToReference(),
                Length,
                Length
            );
        }

        internal PointerArray
        (
            AllocationOwner<TUnmanaged> allocationOwner,
            int length,
            int count
        )
        {
            AllocationOwner = allocationOwner;
            Length = length;
        }

        private readonly bool CheckIndexOutOfRange
        (
            int index
        )
        {
            return index < 0 || index >= Length;
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
            set
            {
                if (CheckIndexOutOfRange(index)) throw new IndexOutOfRangeException();
                *(AllocationOwner.Pointer + index) = value;
            }
        }

        public void Dispose()
        {
            if (Length == 0) return;
            Length = 0;
            AllocationOwner.Dispose();
        }
    }
}