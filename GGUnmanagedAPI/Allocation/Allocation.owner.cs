using System;

namespace UnmanagedAPI
{
    public static partial class Allocation
    {
        public readonly unsafe struct Owner : IAllocation, IDisposable
        {
            public IntPtr IntPtr { get; }
            
            public bool IsNull => IntPtr == IntPtr.Zero;
            
            public static Owner Null => new Owner(IntPtr.Zero);

            
            public Owner
            (
                IntPtr intPtr
            )
            {
                IntPtr = intPtr;
            }

            public Reference<TUnmanaged> ToReference<TUnmanaged>() where TUnmanaged : unmanaged
            {
                return new Reference<TUnmanaged>((TUnmanaged*)IntPtr);
            }

            public TUnmanaged* As<TUnmanaged>() where TUnmanaged : unmanaged
            {
                return (TUnmanaged*)IntPtr;
            }
            
            public static IntPtr operator ~
            (
                Owner reference
            )
            {
                return reference.IntPtr;
            }


            public void Dispose()
            {
                Delete(this);
            }
        }
    }
}