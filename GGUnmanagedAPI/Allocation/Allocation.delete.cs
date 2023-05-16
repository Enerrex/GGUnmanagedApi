using System;
using UnmanagedAPI.Pointer;

namespace UnmanagedAPI
{
    public static unsafe partial class Allocation
    {
        public static void Delete<TUnmanaged>
        (
            AllocationOwner<TUnmanaged> allocationOwner
        ) where TUnmanaged : unmanaged
        {
            Free((IntPtr)allocationOwner.Pointer);
        }

        public static void Delete
        (
            AllocationOwner allocationOwner
        )
        {
            Free(allocationOwner.Pointer);
        }
    }
}