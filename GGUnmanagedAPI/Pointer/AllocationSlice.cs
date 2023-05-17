using System;

namespace UnmanagedAPI.Pointer
{
    public readonly struct AllocationSlice<TUnmanaged> where TUnmanaged : unmanaged
    {
        private readonly Reference<TUnmanaged> _reference;
        public readonly int Length { get; }

        public AllocationSlice
        (
            Reference<TUnmanaged> reference,
            int length
        )
        {
            _reference = reference;
            Length = length;
        }

        // Indexer with bounds checking.
        public TUnmanaged this
        [
            int index
        ]
        {
            get
            {
                if (index < 0 || index >= Length) throw new IndexOutOfRangeException();
                return _reference[index];
            }
            set
            {
                if (index < 0 || index >= Length) throw new IndexOutOfRangeException();
                _reference[index] = value;
            }
        }
    }
}