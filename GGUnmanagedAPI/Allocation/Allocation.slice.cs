using System;

namespace UnmanagedAPI
{
    public static partial class Allocation
    {
        public readonly struct Slice<TUnmanaged> where TUnmanaged : unmanaged
        {
            private readonly Reference<TUnmanaged> _reference;
            public static Slice<TUnmanaged> Null => new Slice<TUnmanaged>(Reference<TUnmanaged>.Null, -1);
            public int Length { get; }

            public Slice
            (
                Reference<TUnmanaged> reference,
                int length
            )
            {
                _reference = reference;
                Length = length;
            }

            public unsafe Slice
            (
                Reference<TUnmanaged> reference,
                int startIndex,
                int length
            )
            {
                _reference = new Reference<TUnmanaged>(~reference + startIndex);
                Length = length;
            }
            
            public SliceRO<TUnmanaged> GetReadOnly()
            {
                return new SliceRO<TUnmanaged>(_reference, Length);
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
                set
                {
                    if (index < 0 || index >= Length) throw new IndexOutOfRangeException();
                    *(_reference.Pointer + index) = value;
                }
            }
        }
    }
}