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
    }
}