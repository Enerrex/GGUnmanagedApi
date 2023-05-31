using System;
using UnmanagedAPI;

namespace UnmanagedCore.Containers
{
    /// <summary>
    ///     WARN: This struct has a functionality to expand infinitely as long as elements are added.
    ///     Every time length is reached, the array is reallocated at ~twice the previous size.
    /// </summary>
    /// <typeparam name="TUnmanaged"></typeparam>
    public unsafe struct PointerArray<TUnmanaged> : IDisposable where TUnmanaged : unmanaged
    {
        private Allocation.Owner<TUnmanaged> Owner { get; }
        public Allocation.Reference<TUnmanaged> Reference => Owner.ToReference();

        public int Length { get; private set; }

        public PointerArray
        (
            int length
        ) : this()
        {
            if (length <= 0) throw new ArgumentException();
            Length = length;
            Owner = Allocation.Create<TUnmanaged>(length);
        }

        public PointerArray
        (
            TUnmanaged valueIn
        ) : this(1)
        {
            Owner.Pointer[0] = valueIn;
        }

        public PointerArray
        (
            PointerArray<TUnmanaged> copiedList
        ) : this(copiedList.Length)
        {
            Owner = Allocation.Copy
            (
                copiedList.Owner.ToReference(),
                Length,
                Length
            );
        }

        internal PointerArray
        (Allocation.Owner<TUnmanaged> owner,
            int length,
            int count
        )
        {
            Owner = owner;
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
                return *(Owner.Pointer + index);
            }
            set
            {
                if (CheckIndexOutOfRange(index)) throw new IndexOutOfRangeException();
                *(Owner.Pointer + index) = value;
            }
        }

        public void Dispose()
        {
            if (Length == 0) return;
            Length = 0;
            Owner.Dispose();
        }
    }
}