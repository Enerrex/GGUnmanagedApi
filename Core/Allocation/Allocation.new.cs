using System;

namespace GGUnmanagedApi.Core
{
    public static unsafe partial class Allocation
    {
        public static Owner<TUnmanaged> AllocateNew<TUnmanaged>
        (
            int length = 1
        ) where TUnmanaged : unmanaged
        {
            return new Owner<TUnmanaged>
            (
                (TUnmanaged*) Malloc
                (
                    SizeOf<TUnmanaged>() * length,
                    AlignOf<TUnmanaged>()
                )
            );
        }
    }
}