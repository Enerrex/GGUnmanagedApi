using System;
using API.Pointer;

namespace Allocation
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
        
        public static void Delete<TOwner, TUnmanaged>
        (
            TOwner allocationOwner
        ) where TOwner : unmanaged, IOwner<TUnmanaged> where TUnmanaged : unmanaged
        {
            Free(allocationOwner.ToReference());
        }
        
        public static void Delete<TOwner>
        (
            TOwner allocationOwner
        ) where TOwner : unmanaged, IOwner<TOwner>
        {
            Free(allocationOwner.ToReference());
        }
    }
}