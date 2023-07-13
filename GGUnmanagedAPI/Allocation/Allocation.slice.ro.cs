using System;

namespace UnmanagedAPI
{
    public static partial class Allocation
    {
        // ReSharper disable once InconsistentNaming
        public readonly struct SliceRO<TUnmanaged> where TUnmanaged : unmanaged
        {
            private readonly Reference<TUnmanaged> _reference;
            public static SliceRO<TUnmanaged> Null => new SliceRO<TUnmanaged>(Reference<TUnmanaged>.Null, -1);
            public int Length { get; }

            public SliceRO
            (
                Reference<TUnmanaged> reference,
                int length
            )
            {
                _reference = reference;
                Length = length;
            }

            public unsafe SliceRO
            (
                Reference<TUnmanaged> reference,
                int startIndex,
                int length
            )
            {
                _reference = new Reference<TUnmanaged>(~reference + startIndex);
                Length = length;
            }

            // Indexer with bounds checking.
            public unsafe TUnmanaged this
            [
                int index
            ]
            {
                get
                {
                    if (index < 0 || index >= Length) throw new IndexOutOfRangeException();
                    return *(_reference.Pointer + index);

                }
            }
        }
    }
}