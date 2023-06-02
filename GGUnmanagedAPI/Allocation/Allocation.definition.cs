using System;

namespace UnmanagedAPI
{
    public static unsafe partial class Allocation
    {
        public interface IAllocation
        {
            public IntPtr IntPtr { get; }
            public bool IsNull { get; }
            
        }
        
        public interface IAllocationGeneric<TUnmanaged> : IAllocation where TUnmanaged : unmanaged
        {
            public TUnmanaged* ToPointer
            (
                int index = 0
            );
        }
    }
}