using System;
using UnmanagedAPI.Iterator;

namespace UnmanagedCore.Containers.Iterators.Extensions
{
    public static class ArrayIteratorExtensions
    {
        public static void DisposeAll<TUnmanaged>
        (
            this ArrayIterator<TUnmanaged> items
        )
            where TUnmanaged : unmanaged, IDisposable
        {
            while (items.MoveNext().Success)
            {
                items.Current.Dispose();
            }
        }
    }
}