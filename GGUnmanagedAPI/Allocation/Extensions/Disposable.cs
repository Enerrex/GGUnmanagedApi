using System;

namespace UnmanagedAPI.Extensions
{
    public static class Disposable
    {
        public static unsafe void DisposeUnderlying<TUnmanaged>
        (
            this UnmanagedAPI.Allocation.Reference<TUnmanaged> item
        )
            where TUnmanaged : unmanaged, IDisposable
        {
            item.Pointer->Dispose();
        }
        
        public static unsafe void DisposeUnderlying<TUnmanaged>
        (
            this UnmanagedAPI.Allocation.Owner<TUnmanaged> item
        )
            where TUnmanaged : unmanaged, IDisposable
        {
            item.Pointer->Dispose();
        }
        
        public static unsafe void DisposeUnderlying<TUnmanaged>
        (
            this UnmanagedAPI.Allocation.Owner item
        )
            where TUnmanaged : unmanaged, IDisposable
        {
            item.As<TUnmanaged>()->Dispose();
        }
        
        public static unsafe void DisposeWithUnderlying<TUnmanaged>
        (
            this UnmanagedAPI.Allocation.Owner<TUnmanaged> item
        )
            where TUnmanaged : unmanaged, IDisposable
        {
            item.Pointer->Dispose();
            item.Dispose();
        }
        
        public static unsafe void DisposeWithUnderlying<TUnmanaged>
        (
            this UnmanagedAPI.Allocation.Owner item
        )
            where TUnmanaged : unmanaged, IDisposable
        {
            item.As<TUnmanaged>()->Dispose();
            item.Dispose();
        }
    }
}