using System;
using System.Runtime.InteropServices;

namespace GGUnmanagedApi.Core
{
    public static class Allocation
    {
        public static IntPtr Malloc(long size, int alignment)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException("size", "Size must be greater than zero.");
            }

            if (alignment <= 0 || (alignment & (alignment - 1)) != 0)
            {
                throw new ArgumentException("Alignment must be a power of 2.", "alignment");
            }

            IntPtr memory_pointer = Marshal.AllocHGlobal(new IntPtr(size + alignment - 1));

            if (memory_pointer == IntPtr.Zero)
            {
                throw new OutOfMemoryException("Failed to allocate unmanaged memory.");
            }

            IntPtr aligned_memory_pointer = new IntPtr(((long)memory_pointer + alignment - 1) & ~(alignment - 1));

            return aligned_memory_pointer;
        }

        public static void Free(IntPtr memoryPointer)
        {
            if (memoryPointer == IntPtr.Zero)
            {
                throw new ArgumentException("Cannot free a zero memory pointer.", "memoryPointer");
            }

            Marshal.FreeHGlobal(memoryPointer);
        }
    }
}