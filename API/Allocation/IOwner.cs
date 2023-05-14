using System;

namespace API.Allocation
{
    public interface IOwner<TUnmanaged> : IDisposable where TUnmanaged : unmanaged
    {
    }
}