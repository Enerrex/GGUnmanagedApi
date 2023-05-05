using GGUnmanagedApi.Core;

namespace GGUnmanagedApi.API.Iterator
{
    public interface IUnmanagedIteratorWithReset<TNode> : IWithReset, IUnmanagedIterator<TNode> where TNode : unmanaged
    {

    }
}