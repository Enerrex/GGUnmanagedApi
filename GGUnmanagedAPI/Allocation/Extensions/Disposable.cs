using System;

namespace UnmanagedAPI.Extensions
{
    public static class Disposable
    {
        public static unsafe void DisposeUnderlying<TUnmanaged>
        (
            this Allocation.Reference<TUnmanaged> item
        )
            where TUnmanaged : unmanaged, IDisposable
        {
            if (item.IsNull) return;
            item.Pointer->Dispose();
        }
        
        public static unsafe void DisposeUnderlying<TUnmanaged>
        (
            this Allocation.Owner<TUnmanaged> item
        )
            where TUnmanaged : unmanaged, IDisposable
        {
            if (item.IsNull) return;
            item.Pointer->Dispose();
        }
        
        public static unsafe void DisposeUnderlying<TUnmanaged>
        (
            this Allocation.Owner item
        )
            where TUnmanaged : unmanaged, IDisposable
        {
            if (item.IsNull) return;
            item.As<TUnmanaged>()->Dispose();
        }
        
        public static unsafe void DisposeWithUnderlying<TUnmanaged>
        (
            this Allocation.Owner<TUnmanaged> item
        )
            where TUnmanaged : unmanaged, IDisposable
        {
            if (item.IsNull) return;
            item.Pointer->Dispose();
            item.Dispose();
        }
        
        public static unsafe void DisposeWithUnderlying<TUnmanaged>
        (
            this Allocation.Owner item
        )
            where TUnmanaged : unmanaged, IDisposable
        {
            if (item.IsNull) return;
            item.As<TUnmanaged>()->Dispose();
            item.Dispose();
        }
    }
}