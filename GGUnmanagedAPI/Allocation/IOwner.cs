using System;
using UnmanagedAPI.Pointer;

namespace UnmanagedAPI
{
    public interface IOwner<TUnmanaged> : IDisposable where TUnmanaged : unmanaged
    {
        public AllocationReference<TUnmanaged> ToReference();
    }
}