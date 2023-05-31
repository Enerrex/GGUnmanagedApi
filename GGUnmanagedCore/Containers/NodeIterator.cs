using System;
using UnmanagedAPI;
using UnmanagedAPI.Iterator;

namespace UnmanagedCore.Containers
{
    // Iterator for LinkedNode<T>
    // Starts at the head of the list and iterates through each node
    public unsafe struct NodeIterator<T> : IUnmanagedIteratorWithIndex<T> where T : unmanaged
    {
        private int _index;
        private Allocation.Reference<LinkedNode<T>> _currentNode;

        public T Current => _currentNode.Pointer->Value;

        public NodeIterator
        (
            Allocation.Reference<LinkedNode<T>> head
        )
        {
            _currentNode = head;
            _index = -1;
        }

        public MoveNextResult MoveNext()
        {
            if (_index != -1)
            {
                _currentNode = _currentNode.Pointer->Next;
            }
            
            if (!_currentNode.IsNull)
            {
                // Return true, and the current index
                // Increment the index after returning
                return (true, _index++);
            }

            return (false, _index);
        }
    }
}