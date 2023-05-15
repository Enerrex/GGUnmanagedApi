using System;
using System.Runtime.InteropServices;

namespace API.Allocation
{
    public static partial class Allocation
    {
        private static void Free
        (
            IntPtr memoryPointer
        )
        {
            if (memoryPointer == IntPtr.Zero) throw new ArgumentException("Cannot free a zero memory pointer.", "memoryPointer");

            Marshal.FreeHGlobal(memoryPointer);
        }
    }
}