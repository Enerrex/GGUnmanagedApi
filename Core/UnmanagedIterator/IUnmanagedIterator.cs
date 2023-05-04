namespace GGUnmanagedApi
{
    public interface IUnmanagedIterator<TNode> where TNode : unmanaged
    {
        TNode Current { get; }
        bool MoveNext();
    }
}