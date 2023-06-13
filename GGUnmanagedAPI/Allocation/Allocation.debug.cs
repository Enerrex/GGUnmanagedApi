using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace UnmanagedAPI
{
    public static partial class Allocation
    {
#if ALLOC_DEBUG
        public static class Debug
        {
            public enum Event
            {
                Allocation,
                Deallocation,
                Boundary
            }

            public struct AllocationInfo
            {
                public bool Equals(AllocationInfo other)
                {
                    return ID == other.ID;
                }

                public override bool Equals(object? obj)
                {
                    return obj is AllocationInfo other && Equals(other);
                }

                public override int GetHashCode()
                {
                    return (int)ID;
                }
                
                // Sort function, should sort by Pointer and ID such that when sorting a list of allocations, the list
                // will contain AllocationInfo's grouped by pointer in the order of their creation.
                public static int Sort(AllocationInfo a, AllocationInfo b)
                {
                    if (a.Pointer == b.Pointer) return a.ID.CompareTo(b.ID);
                    return a.Pointer.ToInt64().CompareTo(b.Pointer.ToInt64());
                }

                public AllocationInfo(Demarker demarker, IntPtr pointer, Event eventType, string callingFunction = "")
                {
                    Demarker = demarker;
                    Pointer = pointer;
                    Event = eventType;
                    CallingFunction = callingFunction;
                    ID = NextID++;
                }

                public Demarker Demarker;
                public IntPtr Pointer;
                public Event Event;
                public string CallingFunction;
                public uint ID;

                public static uint NextID = 0;

                // Equality is based on ID;
                public static bool operator ==(AllocationInfo a, AllocationInfo b)
                {
                    return a.ID == b.ID;
                }

                public static bool operator !=(AllocationInfo a, AllocationInfo b)
                {
                    return !(a == b);
                }

                public override string ToString()
                {
                    return $"{Event.ToString()} {Pointer} {Demarker.Name}";
                }
            }

            public struct Demarker
            {
                public string Name;
                public int Index;

                public override string ToString()
                {
                    return $"{Name}";
                }
            }


            // String name, int index of starting point in AllocationStack
            public static readonly List<Demarker> DemarkerStack = new List<Demarker>(new[]
            {
                new Demarker { Name = "Start", Index = 0 }
            });

            public static readonly List<AllocationInfo> AllocationEventStack = new List<AllocationInfo>
            (
                new[]
                {
                    new AllocationInfo(DemarkerStack[0], IntPtr.Zero, Event.Boundary)
                }
            );

            public static readonly HashSet<IntPtr> AllocatedPointers = new HashSet<IntPtr>();
            public static readonly Dictionary<IntPtr, int> AllocationMap = new Dictionary<IntPtr, int>();
            public static readonly Dictionary<IntPtr, int> DeallocationMap = new Dictionary<IntPtr, int>();


            public static List<AllocationInfo> GetEventsInBoundary(string name, Event? eventType)
            {
                return GetEventsInBoundary(GetDemarker(name), eventType);
            }

            public static List<AllocationInfo> GetEventsInBoundary(Demarker demarker, Event? eventType)
            {
                var events = new List<AllocationInfo>();

                // Return empty if demarker is not found at all
                if (DemarkerStack.FindIndex(d => d.Name == demarker.Name) == -1) return events;

                var startIndex = demarker.Index + 1;
                // Add the boundary event
                events.Add(AllocationEventStack[startIndex - 1]);
                // First check if the current demarker has any events
                // If the AllocationEventStack 
                if (startIndex == AllocationEventStack.Count) return events;

                // Add all events until the next boundary event
                for (var event_ix = startIndex; event_ix < AllocationEventStack.Count; event_ix++)
                {
                    var allocationInfo = AllocationEventStack[event_ix];
                    if (allocationInfo.Event == Event.Boundary) break;
                    if (eventType != null && allocationInfo.Event != eventType) continue;
                    events.Add(allocationInfo);
                }

                return events;
            }

            public static readonly Demarker NEVER = new Demarker { Name = "NEVER", Index = -1 };

            public struct AllocationLife
            {
                public IntPtr Pointer;
                public Demarker Start;
                public Demarker End;
            }

            public static List<AllocationLife> GetLifeTimes(params long[] targets)
            {
                // Convert longs to IntPtrs
                var pointers = targets.Select(l => new IntPtr(l)).ToArray();
                return GetLifeTimes(pointers);
            }

            // Get the lifetime of every allocation
            // A lifetime is an AllocationLife object where we record the start and end demarker
            // Each IntPtr can have multiple, non-overlapping lifetimes
            public static List<AllocationLife> GetLifeTimes(params IntPtr[] targets)
            {
                // There will be a minimum of 1 lifetime per pointer
                // but the lifetime may never end and will have its End demarker set to NEVER
                var lifetimes = new List<AllocationLife>();

                var pointers = targets.Length == 0 ? AllocationMap.Keys.ToList() : targets.ToList();
                // Sort by pointer
                pointers.Sort((a, b) => a.ToInt64().CompareTo(b.ToInt64()));

                // Copy AllocationEventStack
                var sortedEvents = AllocationEventStack.ToList();
                // Remove all events that are not in the target list -- this will also remove the boundary events
                sortedEvents.RemoveAll(e => !pointers.Contains(e.Pointer));
                // Sort by pointer and ID
                sortedEvents.Sort(AllocationInfo.Sort);
                
                // No matching events
                if (sortedEvents.Count == 0) return lifetimes;

                // Iterate through all events in 2's
                // The first event will always be an allocation
                // The second event will either be an allocation or a deallocation
                // If the second event is an allocation, we have reached the end of the current pointer's lifetimes
                var start_ix = 0;
                var event_ix = 1;

                // Only one event in the list
                if (event_ix >= sortedEvents.Count)
                {
                    lifetimes.Add
                    (
                        new AllocationLife
                        {
                            Pointer = sortedEvents[start_ix].Pointer,
                            Start = sortedEvents[start_ix].Demarker,
                            End = NEVER
                        }
                    );
                    return lifetimes;
                }

                AllocationInfo start_event;
                AllocationInfo next_event;
                while (event_ix < sortedEvents.Count)
                {
                    start_event = sortedEvents[start_ix];
                    next_event = sortedEvents[event_ix];

                    if (start_event.Pointer == next_event.Pointer)
                    {
                        // If the next event is a deallocation, we have reached the end of the current lifetime
                        // Add the current lifetime to the list
                        lifetimes.Add
                        (
                            new AllocationLife
                            {
                                Pointer = start_event.Pointer,
                                Start = start_event.Demarker,
                                End = next_event.Demarker
                            }
                        );
                        start_ix += 2;
                        event_ix += 2;
                    }

                    // Current event is not matched to the current pointer
                    // This means that the current pointer has no deallocation event
                    // Add the current lifetime to the list
                    if (start_event.Pointer != next_event.Pointer)
                    {
                        lifetimes.Add
                        (
                            new AllocationLife
                            {
                                Pointer = start_event.Pointer,
                                Start = start_event.Demarker,
                                End = NEVER
                            }
                        );
                        start_ix++;
                        event_ix++;
                    }
                }
                
                if (start_ix < sortedEvents.Count)
                {
                    start_event = sortedEvents[start_ix];
                    lifetimes.Add
                    (
                        new AllocationLife
                        {
                            Pointer = start_event.Pointer,
                            Start = start_event.Demarker,
                            End = NEVER
                        }
                    );
                }
                
                return lifetimes;
            }

            public static List<AllocationInfo> GetEventsForPointer(IntPtr pointer)
            {
                return AllocationEventStack.FindAll((info => info.Pointer == pointer));
            }

            public static Demarker GetDemarker(string name)
            {
                return DemarkerStack.Find(demarker => demarker.Name == name);
            }

            public static void RegisterBoundary(string name)
            {
                var demarker = new Demarker { Name = name, Index = AllocationEventStack.Count };
                AllocationEventStack.Add
                (
                    CreateInfo(IntPtr.Zero, Event.Boundary, demarker)
                );
                DemarkerStack.Add(demarker);
            }

            public static void RegisterAllocation(IntPtr pointer)
            {
                // Register pointer in map of all allocated pointers
                AllocatedPointers.Add(pointer);
                // Register the allocation event
                AllocationEventStack.Add
                (
                    CreateInfo(pointer, Event.Allocation)
                );

                // Ensure deallocation map has the pointer.
                DeallocationMap.TryAdd(pointer, 0);
                if (AllocationMap.TryAdd(pointer, 1)) return;

                // The allocation map already has the pointer from a previous use of the address.
                // Increment the allocation count.
                AllocationMap[pointer]++;
            }

            public static void RegisterDeallocation(IntPtr pointer)
            {
                // Deregister pointer in map of all allocated pointers
                AllocatedPointers.Remove(pointer);
                // Register the deallocation event
                AllocationEventStack.Add
                (
                    CreateInfo(pointer, Event.Deallocation)
                );

                // The initial allocation should prepopulate the deallocation map
                // with the pointer and a reference count of 0 such that the first
                // deallocation will increment the reference count to 1.
                try
                {
                    DeallocationMap[pointer]++;
                }
                catch (Exception e)
                {
                    // Probably attempting to deallocate something that was never allocated with
                    // our library
                    var a = e;
                    string c = a.ToString();
                    // rethrow the exception
                    throw;
                }
            }

            private static AllocationInfo CreateInfo(IntPtr pointer, Event eventType, Demarker? demarker = null)
            {
                // Construct a string of the form "DeclaringType -> Method Name"
                var calling_function = new StackTrace().GetFrame(5).GetMethod();
                var calling_function_name = $"{calling_function.DeclaringType} -> {calling_function.Name}";
                return new AllocationInfo(demarker ?? GetMostRecentDemarker(), pointer, eventType,
                    calling_function_name);
            }

            private static Demarker GetMostRecentDemarker()
            {
                return DemarkerStack[^1];
            }

            // List of tuples of (IntPtr, AllocationCount, DeallocationCount)
            public static List<(IntPtr, int, int)> GetNotDeallocated()
            {
                var result = new List<(IntPtr, int, int)>();
                foreach (var elem in AllocationMap)
                {
                    var matched = DeallocationMap[elem.Key];
                    if (elem.Value == matched) continue;
                    result.Add((elem.Key, elem.Value, matched));
                }

                return result;
            }
        }
#endif
    }
}