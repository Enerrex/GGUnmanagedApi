using System;
using System.Runtime.InteropServices;
using UnmanagedAPI.Pointer;

namespace UnmanagedAPI
{
    public static partial class Allocation
    {
        private static PointerOwner Malloc
        (
            long size,
            int alignment
        )
        {
            if (size <= 0) throw new ArgumentOutOfRangeException("size", "Size must be greater than zero.");

            if (alignment <= 0 || (alignment & (alignment - 1)) != 0)
                throw new ArgumentException("Alignment must be a power of 2.", "alignment");

            var memory_pointer = Marshal.AllocHGlobal(new IntPtr(size + alignment - 1));

            if (memory_pointer == IntPtr.Zero) throw new OutOfMemoryException("Failed to allocate unmanaged memory.");

            var aligned_memory_pointer = new IntPtr
            (
                ((long)memory_pointer + alignment - 1) & ~(alignment - 1)
            );

            return new PointerOwner(aligned_memory_pointer);
        }

        /// <summary>
        ///     Allocates unmanaged memory for "length" number of elements of type TUnmanaged.
        /// </summary>
        /// <param name="length">Number of elements to allocate space for.</param>
        /// <typeparam name="TUnmanaged">Type of element to be allocated</typeparam>
        /// <returns></returns>
        private static PointerOwner Malloc<TUnmanaged>
        (
            int length
        ) where TUnmanaged : unmanaged
        {
            var alignment = AlignOf<TUnmanaged>();
            var size = SizeOf<TUnmanaged>() * length;
            return Malloc(size, alignment);
        }
    }
}