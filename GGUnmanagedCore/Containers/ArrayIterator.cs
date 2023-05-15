using UnmanagedAPI.Iterator;
using UnmanagedAPI.Pointer;

namespace Core.Containers
{
    public struct ArrayIterator<TUnmanaged> : IUnmanagedIteratorWithIndex<TUnmanaged> where TUnmanaged : unmanaged
    {
        private int _index;
        private readonly AllocationSlice<TUnmanaged> _allocationReference;
        public TUnmanaged Current => _allocationReference[_index];

        public ArrayIterator
        (
            AllocationSlice<TUnmanaged> head
        )
        {
            _allocationReference = head;
            _index = -1;
        }

        public MoveNextResult MoveNext()
        {
            if (_index < _allocationReference.Length - 1)
            {
                _index++;
                return (_index < _allocationReference.Length, _index);
            }

            return (false, _index);
        }
    }
}