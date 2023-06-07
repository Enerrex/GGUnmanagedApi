using System;
using UnmanagedAPI;
using UnmanagedAPI.Containers;

namespace UnmanagedCore.Containers
{
    /// <summary>
    ///     WARN: This struct has a functionality to expand infinitely as long as elements are added.
    ///     Every time length is reached, the array is reallocated at ~twice the previous size.
    /// </summary>
    /// <typeparam name="TUnmanaged"></typeparam>
    public unsafe struct PointerArray<TUnmanaged> : IPointerStorage<TUnmanaged>, IDisposable where TUnmanaged : unmanaged
    {
        private Allocation.Owner<TUnmanaged> _owner;
        private int _length;
        private readonly Allocation.Owner<TUnmanaged> Owner => _owner;
        public Allocation.Reference<TUnmanaged> Reference => Owner.ToReference();
        public int Length
        {
            readonly get => _length;
            private set => _length = value;
        }

        public PointerArray
        (
            int length
        ) : this()
        {
            if (length <= 0) throw new ArgumentException();
            Length = length;
            _owner = Allocation.Create<TUnmanaged>(length);
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
            _owner = Allocation.Copy
            (
                copiedList.Owner.ToReference(),
                Length,
                Length
            );
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
        
        public Allocation.Slice<TUnmanaged> GetSlice
        (
            int startIndex = 0,
            int? length = null
        )
        {
            int length_value = length ?? Length - startIndex;
            // List isn't allocated, return Slice for Null
            if (_owner.IsNull) return Allocation.Slice<TUnmanaged>.Null;
            
            // Check if the slice is out of range
            if (CheckIndexOutOfRange(startIndex)) throw new IndexOutOfRangeException();
            if (CheckIndexOutOfRange(startIndex + length_value - 1)) throw new IndexOutOfRangeException();
            return new Allocation.Slice<TUnmanaged>
            (
                Owner.ToReference(),
                startIndex,
                length_value
            );
        }

        public void Dispose()
        {
            if (Length == 0) return;
            Owner.Dispose();
            Length = 0;
            _owner = Allocation.Owner<TUnmanaged>.Null;
        }
    }
}