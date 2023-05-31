using UnmanagedAPI;
using UnmanagedAPI.Iterator;

namespace UnmanagedCore.Containers
{
    public struct ArrayIterator<TUnmanaged> : IUnmanagedIteratorWithIndex<TUnmanaged> where TUnmanaged : unmanaged
    {
        private int _index;
        // Provides bounds checking on the pointer.
        private readonly Allocation.Slice<TUnmanaged> _reference;
        public TUnmanaged Current => _reference[_index];

        public ArrayIterator
        (
            Allocation.Slice<TUnmanaged> head
        )
        {
            _reference = head;
            _index = -1;
        }

        public MoveNextResult MoveNext()
        {
            if (_index < _reference.Length - 1)
            {
                _index++;
                return (_index < _reference.Length, _index);
            }

            return (false, _index);
        }
    }
}