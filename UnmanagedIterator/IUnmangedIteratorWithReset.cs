namespace GGUnmanagedApi
{
    public interface IUnmanagedIteratorWithReset<TNode> : IWithReset, IUnmanagedIterator<TNode> where TNode : unmanaged
    {

    }
}