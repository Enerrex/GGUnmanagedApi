using System;
using GGUnmanagedApi.Core.Pointer;

namespace GGUnmanagedApi.Core
{
    public static unsafe partial class Allocation
    {
        /// <summary>
        ///     Copies the memory from the source array to a new array of the target size.
        /// </summary>
        /// <param name="sourcePointer">The source array.</param>
        /// <param name="sourceLength">The sourceLength of the source array.</param>
        /// <param name="targetLength">The sourceLength of the target array.</param>
        /// <typeparam name="TUnmanaged">The type of the array.</typeparam>
        /// <returns>An AllocationOwner object to the target array.</returns>
        /// <exception cref="ArgumentException">Thrown when the target sourceLength is smaller than the source sourceLength.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the target sourceLength or the source sourceLength is smaller than 1.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the source array is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the target array is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the size is negative.</exception>
        public static AllocationOwner<TUnmanaged> Copy<TUnmanaged>
        (
            AllocationReference<TUnmanaged> sourcePointer,
            int sourceLength,
            int targetLength
        ) where TUnmanaged : unmanaged
        {
            if (targetLength < sourceLength)
            {
                throw new ArgumentException();
            }
            
            // Allocate the memory for the target array.
            var target_allocation = AllocateNew<TUnmanaged>(targetLength);
            CopyTo
            (
                sourcePointer,
                sourceLength,
                target_allocation.ToReference(),
                targetLength
            );

            return target_allocation;
        }

        /// <summary>
        ///     Copies the memory from the source array to the target array.
        /// </summary>
        /// <param name="sourceAllocation">The source array.</param>
        /// <param name="sourceLength">The sourceLength of the source array.</param>
        /// <param name="targetAllocation">The target array.</param>
        /// <param name="targetLength">The sourceLength of the target array.</param>
        /// <typeparam name="TUnmanaged">The type of the array.</typeparam>
        /// <returns>An AllocationOwner object to the target array.</returns>
        public static void CopyTo<TUnmanaged>
        (
            in AllocationReference<TUnmanaged> sourceAllocation,
            int sourceLength,
            in AllocationReference<TUnmanaged> targetAllocation,
            int targetLength
        ) where TUnmanaged : unmanaged
        {
            if (targetLength < sourceLength)
            {
                throw new ArgumentException();
            }

            // Size of the type.
            var size = SizeOf<TUnmanaged>();
            // Target and Source sourceLength must be greater than 0.
            if (targetLength <= 0) throw new ArgumentOutOfRangeException();
            if (sourceLength <= 0) throw new ArgumentOutOfRangeException();
            // Copy bytes from source to target.
            MemCopy
            (
                sourceAllocation,
                targetAllocation,
                size * sourceLength
            );
        }

        private static void MemCopy
        (
            IntPtr source,
            IntPtr target,
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

            byte* src = (byte*)source;
            byte* dest = (byte*)target;

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