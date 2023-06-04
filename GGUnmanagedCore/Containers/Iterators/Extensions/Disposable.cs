using System;
using UnmanagedAPI.Iterator;

namespace UnmanagedCore.Containers.Iterators.Extensions
{
    public static class Disposable
    {
        public static void Dispose<TUnmanaged>
        (
            this TUnmanaged item
        )
            where TUnmanaged : unmanaged, IDisposable
        {
            item.Dispose();
        }

        public static void Dispose<TUnmanaged, TIterator>
        (
            this TIterator items
        )
            where TUnmanaged : unmanaged, IDisposable
            where TIterator : unmanaged, IUnmanagedIterator<TUnmanaged>
        {
            while (items.MoveNext())
            {
                
            }
        }
    }
}