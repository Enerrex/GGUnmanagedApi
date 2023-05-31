﻿using System;

namespace UnmanagedAPI
{
    public static partial class Allocation
    {
        public readonly unsafe struct Reference<TUnmanaged> : IAllocation where TUnmanaged : unmanaged
        {
            // ReSharper disable once MemberCanBePrivate.Global
            public TUnmanaged* Pointer { get; }
            public IntPtr IntPtr => (IntPtr)Pointer;

            public bool IsNull => (IntPtr)Pointer == IntPtr.Zero;

            public Reference
            (
                Owner<TUnmanaged> pointer
            )
            {
                Pointer = pointer;
            }

            public Reference
            (
                TUnmanaged* pointer
            )
            {
                Pointer = pointer;
            }

            public TUnmanaged this
            [
                int index
            ]
            {
                get => *(Pointer + index);
                set => *(Pointer + index) = value;
            }

            public TUnmanaged* ToPointer
            (
                int index = 0
            )
            {
                return Pointer + index;
            }

            #region OPERATORS

            public static implicit operator IntPtr
            (
                Reference<TUnmanaged> reference
            )
            {
                return (IntPtr)reference.Pointer;
            }

            public static implicit operator TUnmanaged*
            (
                Reference<TUnmanaged> pointer
            )
            {
                return pointer.Pointer;
            }

            // Operator to convert from TUnmanaged* to Reference<TUnmanaged>
            public static implicit operator Reference<TUnmanaged>
            (
                TUnmanaged* pointer
            )
            {
                return new Reference<TUnmanaged>(pointer);
            }

            // Operator to convert from Owner<TUnmanaged> to Reference<TUnmanaged>
            public static implicit operator Reference<TUnmanaged>
            (
                Owner<TUnmanaged> pointer
            )
            {
                return new Reference<TUnmanaged>(pointer);
            }

            // Override ~ operator to convert from Reference<TUnmanaged> to TUnmanaged*
            public static TUnmanaged* operator ~
            (
                Reference<TUnmanaged> reference
            )
            {
                return reference.Pointer;
            }

            #endregion
        }
    }
}