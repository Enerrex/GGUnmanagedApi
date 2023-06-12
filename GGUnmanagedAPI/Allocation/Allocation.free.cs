using System;
using System.Runtime.InteropServices;

namespace UnmanagedAPI
{
    public static partial class Allocation
    {
        private static void Free
        (
            IntPtr memoryPointer
        )
        {
            if (memoryPointer == IntPtr.Zero)
                throw new ArgumentException("Cannot free a zero memory pointer.", "memoryPointer");

#if ALLOC_DEBUG
            Debug.RegisterDeallocation(memoryPointer);
#endif
            Marshal.FreeHGlobal(memoryPointer);
        }
    }
}