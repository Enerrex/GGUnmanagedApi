using System;
using Core.Pointer;

namespace Core
{
    public static unsafe partial class Allocation
    {
        public static void Delete<T>
        (
            AllocationOwner<T> allocationOwner
        ) where T : unmanaged
        {
            Free((IntPtr)allocationOwner.Pointer);
        }
    }
}