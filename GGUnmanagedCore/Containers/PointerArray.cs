using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnmanagedAPI;
using UnmanagedAPI.Containers;
using UnmanagedCore.Debug.Proxies;

namespace UnmanagedCore.Containers
{
    [DebuggerTypeProxy(typeof(PointerArrayProxy<>))]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            readonly get
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
        
        public readonly Allocation.Slice<TUnmanaged> GetSlice
        (
            int startIndex = 0,
            int? length = null
        )
        {
            // List isn't allocated, return Slice for Null
            if (_owner.IsNull) return Allocation.Slice<TUnmanaged>.Null;
            int length_value = length ?? Length - startIndex;

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