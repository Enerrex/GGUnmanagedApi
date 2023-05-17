using System;

namespace UnmanagedAPI
{
    public static partial class Allocation
    {
        public readonly unsafe struct Owner
        {
            public IntPtr Pointer { get; }

            public bool IsNull => Pointer == IntPtr.Zero;

            public Owner
            (
                IntPtr pointer
            )
            {
                Pointer = pointer;
            }

            public Reference<TUnmanaged> ToReference<TUnmanaged>() where TUnmanaged : unmanaged
            {
                return new Reference<TUnmanaged>((TUnmanaged*)Pointer);
            }

            public TUnmanaged* As<TUnmanaged>() where TUnmanaged : unmanaged
            {
                return (TUnmanaged*)Pointer;
            }
            
            public static IntPtr operator ~
            (
                Owner reference
            )
            {
                return reference.Pointer;
            }


            public void Dispose()
            {
                Delete(this);
            }
            
            
        }
    }
}