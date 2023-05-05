using System;

namespace GGUnmanagedApi.Core
{
    public static unsafe partial class Allocation
    {
        public readonly struct Owner<TUnmanaged> : IDisposable where TUnmanaged : unmanaged
        {
            public Owner
            (
                TUnmanaged* pointer
            )
            {
                Pointer = pointer;
            }

            // ReSharper disable once MemberCanBePrivate.Global
            public TUnmanaged* Pointer { get; }

            public void Dispose()
            {
                Delete(this);
            }

            public static implicit operator IntPtr
            (
                Owner<TUnmanaged> owner
            )
            {
                return (IntPtr) owner.Pointer;
            }

            public static implicit operator TUnmanaged*
            (
                Owner<TUnmanaged> pointer
            )
            {
                return pointer.Pointer;
            }
        }
    }
}