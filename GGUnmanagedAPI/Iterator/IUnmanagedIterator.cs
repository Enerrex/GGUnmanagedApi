namespace UnmanagedAPI.Iterator
{
    public interface IUnmanagedIteratorBase<out TNode, out THasNext> where TNode : unmanaged
    {
        TNode Current { get; }
        THasNext MoveNext();
    }

    public interface IUnmanagedIterator<TNode> : IUnmanagedIteratorBase<TNode, bool> where TNode : unmanaged
    {
    }

    public interface IUnmanagedIteratorWithIndex<TNode> : IUnmanagedIteratorBase<TNode, MoveNextResult>
        where TNode : unmanaged
    {
    }
}