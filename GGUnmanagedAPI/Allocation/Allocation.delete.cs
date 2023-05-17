using System;

namespace UnmanagedAPI
{
    public static unsafe partial class Allocation
    {
        public static void Delete<TUnmanaged>
        (
            Owner<TUnmanaged> owner
        ) where TUnmanaged : unmanaged
        {
            Free((IntPtr)owner.Pointer);
        }

        public static void Delete
        (
            Owner owner
        )
        {
            Free(~owner);
        }
    }
}