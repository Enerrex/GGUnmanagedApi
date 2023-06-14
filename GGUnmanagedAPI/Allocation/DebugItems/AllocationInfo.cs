#if ALLOC_DEBUG
using System;
using System.Diagnostics;

namespace UnmanagedAPI.DebugItems
{
    public struct AllocationInfo
    {
        public bool Equals
        (
            AllocationInfo other
        )
        {
            return ID == other.ID;
        }

        public override bool Equals
        (
            object? obj
        )
        {
            return obj is AllocationInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int) ID;
        }

        // Sort function, should sort by Pointer and ID such that when sorting a list of allocations, the list
        // will contain AllocationInfo's grouped by pointer in the order of their creation.
        public static int Sort
        (
            AllocationInfo a,
            AllocationInfo b
        )
        {
            if (a.Pointer == b.Pointer) return a.ID.CompareTo(b.ID);
            return a.Pointer.ToInt64().CompareTo(b.Pointer.ToInt64());
        }

        public AllocationInfo
        (
            Demarker demarker,
            IntPtr pointer,
            Allocation.Debug.Event eventType,
            StackTrace? callingTrace = null
        )
        {
            Demarker = demarker;
            Pointer = pointer;
            Event = eventType;
            CallingTrace = callingTrace;
            ID = NextID++;
        }

        public Demarker Demarker;
        public IntPtr Pointer;
        public Allocation.Debug.Event Event;
        public StackTrace? CallingTrace;
        public uint ID;

        public static uint NextID = 0;

        // Equality is based on ID;
        public static bool operator ==
        (
            AllocationInfo a,
            AllocationInfo b
        )
        {
            return a.ID == b.ID;
        }

        public static bool operator !=
        (
            AllocationInfo a,
            AllocationInfo b
        )
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return $"{Event.ToString()} {Pointer} {Demarker.Name}";
        }
    }
}
#endif