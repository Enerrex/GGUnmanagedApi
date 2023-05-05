using GGUnmanagedApi.API.Iterator;
using GGUnmanagedApi.Core.Iterator;

namespace GGUnmanagedApi.Core.Containers
{
    public unsafe struct NodeIterator<T> : IUnmanagedIteratorWithIndex<T> where T : unmanaged
    {
        private int _index;
        private readonly LinkedNode<T>* _startNode;
        private LinkedNode<T>* _currentNode;
        public T Current => _currentNode->Value;

        public NodeIterator(LinkedNode<T>* head)
        {
            _startNode = head;
            _currentNode = null;
            _index = -1;
        }
        
        bool IUnmanagedIterator<T>.MoveNext()
        {
            return MoveNext().Success;
        }
            
        public MoveNextResult MoveNext()
        {
            if (_currentNode == null)
            {
                _currentNode = _startNode;
            }
            
            if (_currentNode == null) return (false, _index);
            _currentNode = _currentNode->Next;
            _index++;
            return (_currentNode != null, _index);
        }
    }
}