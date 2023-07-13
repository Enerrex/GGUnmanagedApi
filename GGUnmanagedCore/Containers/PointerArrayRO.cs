using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnmanagedAPI;
using UnmanagedAPI.Containers;
using UnmanagedCore.Containers.Iterators.Extensions;
using UnmanagedCore.Debug.Proxies;

namespace UnmanagedCore.Containers
{
    [DebuggerTypeProxy(typeof(PointerArrayProxy<>))]
    public unsafe struct PointerArrayRO<TUnmanaged> : ISliceable<TUnmanaged>, IDisposable where TUnmanaged : unmanaged
    {
        private Allocation.Owner<TUnmanaged> _owner;
        private int _length;
        private readonly Allocation.Owner<TUnmanaged> Owner => _owner;
        private Allocation.Reference<TUnmanaged> Reference => Owner.ToReference();
        public int Length
        {
            readonly get => _length;
            private set => _length = value;
        }

        public PointerArrayRO
        (
            int length
        ) : this()
        {
            if (length <= 0) throw new ArgumentException();
            Length = length;
            _owner = Allocation.Create<TUnmanaged>(length);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly bool CheckIndexOutOfRange
        (
            int index
        )
        {
            return index < 0 || index >= Length;
        }
        
        public Allocation.Slice<TUnmanaged> GetSlice
        (
            int start = 0,
            int? length = null
        )
        {
            // List isn't allocated, return Slice for Null
            if (_owner.IsNull) return Allocation.Slice<TUnmanaged>.Null;
            int length_value = length ?? Length - start;

            // Check if the slice is out of range
            if (CheckIndexOutOfRange(start)) throw new IndexOutOfRangeException();
            if (CheckIndexOutOfRange(start + length_value - 1)) throw new IndexOutOfRangeException();
            return new Allocation.Slice<TUnmanaged>
            (
                Owner.ToReference(),
                start,
                length_value
            );
        }
        
        public void Dispose()
        {
            _owner.Dispose();
        }
    }
}