using GGUnmanagedApi.Core.Iterator;

namespace GGUnmanagedApi.API.Iterator
{
    public interface IUnmanagedIteratorBase<TNode, THasNext> where TNode : unmanaged
    {
        TNode Current { get; }
        THasNext MoveNext();
    }
    
    public interface IUnmanagedIterator<TNode> : IUnmanagedIteratorBase<TNode, bool> where TNode : unmanaged
    {
    }
    
    public interface IUnmanagedIteratorWithIndex<TNode> : IUnmanagedIteratorBase<TNode, MoveNextResult> where TNode : unmanaged
    {
    }
}