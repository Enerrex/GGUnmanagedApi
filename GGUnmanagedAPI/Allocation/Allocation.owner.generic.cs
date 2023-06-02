using System;

namespace UnmanagedAPI
{
    public static partial class Allocation
    {
        public readonly unsafe struct Owner<TUnmanaged> : IDisposable, IAllocationGeneric<TUnmanaged>
            where TUnmanaged : unmanaged
        {
            // ReSharper disable once MemberCanBePrivate.Global
            public TUnmanaged* Pointer { get; }
            public IntPtr IntPtr => (IntPtr)Pointer;
            
            public bool IsNull => (IntPtr)Pointer == IntPtr.Zero;

            public Owner
            (
                TUnmanaged* pointer
            )
            {
                Pointer = pointer;
            }
            
            public TUnmanaged* ToPointer
            (
                int index = 0
            )
            {
                return Pointer + index;
            }

            public Slice<TUnmanaged> GetSlice
            (
                int length
            )
            {
                return new Slice<TUnmanaged>(ToReference(), length);
            }

            public Reference<TUnmanaged> ToReference()
            {
                return new Reference<TUnmanaged>(this);
            }

            public void Dispose()
            {
                Delete(this);
            }

            #region OPERATORS

            public static implicit operator IntPtr
            (
                Owner<TUnmanaged> owner
            )
            {
                return (IntPtr)owner.Pointer;
            }

            public static implicit operator TUnmanaged*
            (
                Owner<TUnmanaged> pointer
            )
            {
                return pointer.Pointer;
            }

            public static implicit operator Reference<TUnmanaged>
            (
                Owner<TUnmanaged> owner
            )
            {
                return owner.ToReference();
            }

            public static TUnmanaged* operator ~
            (
                Owner<TUnmanaged> reference
            )
            {
                return reference.Pointer;
            }

            #endregion
        }
    }
}