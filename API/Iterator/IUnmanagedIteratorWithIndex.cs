using GGUnmanagedApi.Core;
using GGUnmanagedApi.Core.Iterator;

namespace GGUnmanagedApi.API.Iterator
{
    public interface IUnmanagedIteratorWithIndex<TNode> : IUnmanagedIterator<TNode> where TNode : unmanaged
    {
        new MoveNextResult MoveNext();
    }
}