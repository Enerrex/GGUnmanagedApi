using System;
using API.Iterator;
using Core.Pointer;
using GGUnmanagedApi.Core.Iterator;

namespace Core.Containers
{
    public unsafe struct NodeIterator<T> : IUnmanagedIteratorWithIndex<T> where T : unmanaged
    {
        private int _index;
        private readonly AllocationReference<LinkedNode<T>> _startNode;
        private AllocationReference<LinkedNode<T>> _currentNode;
        
        public T Current => _currentNode.Pointer->Value;

        public NodeIterator
        (
            AllocationReference<LinkedNode<T>> head
        )
        {
            _startNode = head;
            _currentNode = default;
            _index = -1;
        }

        public MoveNextResult MoveNext()
        {
            if (_currentNode.IsNull && _index == -1) _currentNode = _startNode;

            if (_currentNode.IsNull) return (false, _index);
            _currentNode = _currentNode.Pointer->Next;
            _index++;
            return (!_currentNode.IsNull, _index);
        }
    }
}