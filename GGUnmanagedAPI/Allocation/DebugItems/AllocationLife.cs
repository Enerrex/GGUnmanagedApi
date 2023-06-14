#if ALLOC_DEBUG
using System;
using System.Diagnostics;

namespace UnmanagedAPI.DebugItems
{
    public struct AllocationLife
    {
        public IntPtr Pointer;
        public Demarker Start;
        public Demarker End;
        public StackTrace? AllocatingTrace;
        public StackTrace? DeallocatingTrace;
    }
}
#endif