namespace GGUnmanagedApi
{
    public interface IUnmanagedIteratorWithIndex<TNode> : IUnmanagedIterator<TNode> where TNode : unmanaged
    {
        new MoveNextResult MoveNext();
    }
}