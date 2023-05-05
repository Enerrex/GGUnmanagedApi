using System;

namespace GGUnmanagedApi.Core
{
    public static unsafe partial class Allocation
    {
        /// <summary>
        ///     Copies the memory from the source array to a new array of the target size.
        /// </summary>
        /// <param name="copiedArray">The source array.</param>
        /// <param name="length">The length of the source array.</param>
        /// <param name="targetLength">The length of the target array.</param>
        /// <typeparam name="TUnmanaged">The type of the array.</typeparam>
        /// <returns>A Owner object to the target array.</returns>
        /// <exception cref="ArgumentException">Thrown when the target length is smaller than the source length.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the target length or the source length is smaller than 1.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the source array is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the target array is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the size is negative.</exception>
        public static Owner<TUnmanaged> Copy<TUnmanaged>
        (
            TUnmanaged* copiedArray,
            int length,
            int targetLength
        ) where TUnmanaged : unmanaged
        {
            if (targetLength < length)
            {
                throw new ArgumentException();
            }
            var size = SizeOf<TUnmanaged>();
            var target = Malloc(size * targetLength, AlignOf<TUnmanaged>());
            if (targetLength <= 0) throw new ArgumentOutOfRangeException();
            if (length <= 0) throw new ArgumentOutOfRangeException();
            MemCopy
            (
                target,
                (IntPtr) copiedArray,
                size * length
            );
            return new Owner<TUnmanaged>((TUnmanaged*) target);
        }

        private static void MemCopy
        (
            IntPtr target,
            IntPtr source,
            long size
        )
        {
            if (target == IntPtr.Zero)
            {
                throw new ArgumentNullException("target", "Target pointer cannot be null.");
            }

            if (target == IntPtr.Zero)
            {
                throw new ArgumentNullException("source", "Source pointer cannot be null.");
            }

            if (size < 0)
            {
                throw new ArgumentOutOfRangeException("size", "Size must be non-negative.");
            }

            byte* src = (byte*) source;
            byte* dest = (byte*) target;

            while (size > 0)
            {
                *dest = *src;
                src++;
                dest++;
                size--;
            }
        }
    }
}