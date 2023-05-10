using System;

namespace GGUnmanagedApi.Core.Pointer
{
    public readonly struct AllocationSlice<TUnmanaged> where TUnmanaged : unmanaged
    {
        private readonly AllocationReference<TUnmanaged> _allocationReference;
        public readonly int Length { get; }
        
        public AllocationSlice
        (
            AllocationReference<TUnmanaged> allocationReference,
            int length
        )
        {
            _allocationReference = allocationReference;
            Length = length;
        }
        
        // Indexer with bounds checking.
        public TUnmanaged this[int index]
        {
            get
            {
                if (index < 0 || index >= Length)
                {
                    throw new IndexOutOfRangeException();
                }
                return _allocationReference[index];
            }
            set
            {
                if (index < 0 || index >= Length)
                {
                    throw new IndexOutOfRangeException();
                }
                _allocationReference[index] = value;
            }
        }
    }
}