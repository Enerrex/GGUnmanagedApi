using System;

namespace UnmanagedAPI
{
    public static partial class Allocation
    {
        public readonly struct Slice<TUnmanaged> where TUnmanaged : unmanaged
        {
            private readonly Reference<TUnmanaged> _reference;
            public readonly int Length { get; }

            public Slice
            (
                Reference<TUnmanaged> reference,
                int length
            )
            {
                _reference = reference;
                Length = length;
            }

            // Indexer with bounds checking.
            public TUnmanaged this
            [
                int index
            ]
            {
                get
                {
                    if (index < 0 || index >= Length) throw new IndexOutOfRangeException();
                    return _reference[index];
                }
                set
                {
                    if (index < 0 || index >= Length) throw new IndexOutOfRangeException();
                    _reference[index] = value;
                }
            }
        }
    }
}