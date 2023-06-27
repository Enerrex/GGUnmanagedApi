using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnmanagedAPI.DebugItems;

namespace UnmanagedAPI
{
    public static partial class Allocation
    {
        public static class Debug
        {
            public enum Event
            {
                Allocation,
                Deallocation,
                Boundary
            }

            private static readonly Demarker NEVER = new Demarker
            {
                Name = "NEVER",
                Index = -1
            };


            // String name, int index of starting point in AllocationStack
            public static readonly List<Demarker> DemarkerStack = new List<Demarker>
            (
                new[]
                {
                    new Demarker
                    {
                        Name = "Start",
                        Index = 0
                    }
                }
            );

            public static readonly List<AllocationInfo> AllocationEventStack = new List<AllocationInfo>
            (
                new[]
                {
                    new AllocationInfo
                    (
                        DemarkerStack[0],
                        IntPtr.Zero,
                        Event.Boundary
                    )
                }
            );

            public static readonly HashSet<long> AllocatedPointers = new HashSet<long>();
            public static readonly Dictionary<long, int> AllocationMap = new Dictionary<long, int>();
            public static readonly Dictionary<long, int> DeallocationMap = new Dictionary<long, int>();

            public static void Reset()
            {
                DemarkerStack.Clear();
                AllocationEventStack.Clear();
                AllocatedPointers.Clear();
                AllocationMap.Clear();
                DeallocationMap.Clear();

                DemarkerStack.Add
                (
                    new Demarker
                    {
                        Name = "Start",
                        Index = 0
                    }
                );
                AllocationEventStack.Add
                (
                    new AllocationInfo
                    (
                        DemarkerStack[0],
                        IntPtr.Zero,
                        Event.Boundary
                    )
                );
            }

            public static List<AllocationInfo> GetEventsInBoundary
            (
                string name,
                Event? eventType
            )
            {
                return GetEventsInBoundary(GetDemarker(name), eventType);
            }

            public static List<AllocationInfo> GetEventsInBoundary
            (
                Demarker demarker,
                Event? eventType
            )
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

            // Get the lifetime of every allocation
            // A lifetime is an AllocationLife object where we record the start and end demarker
            // Each IntPtr can have multiple, non-overlapping lifetimes
            public static List<AllocationLife> GetLifeTimes
            (
                bool excludeDeallocations = false,
                params long[] targets
            )
            {
                // There will be a minimum of 1 lifetime per pointer
                // but the lifetime may never end and will have its End demarker set to NEVER
                var lifetimes = new List<AllocationLife>();

                var pointers = targets.Length == 0 ? AllocationMap.Keys.ToList() : targets.ToList();
                // Sort by pointer
                pointers.Sort
                (
                    (
                        a,
                        b
                    ) => a.CompareTo(b)
                );
                // Remove IntPtr.Zero
                pointers.RemoveAll(p => p == 0);

                // Copy AllocationEventStack
                var sorted_events = AllocationEventStack.ToList();
                // Remove all events that are not in the target list -- this will also remove the boundary events
                sorted_events.RemoveAll(e => !pointers.Contains((long)e.Pointer));
                // Sort by pointer and ID
                sorted_events.Sort(AllocationInfo.Sort);

                // No matching events
                if (sorted_events.Count == 0) return lifetimes;

                // Iterate through all events in 2's
                // The first event will always be an allocation
                // The second event will either be an allocation or a deallocation
                // If the second event is an allocation, we have reached the end of the current pointer's lifetimes
                var start_ix = 0;
                var event_ix = 1;

                // Only one event in the list
                if (event_ix >= sorted_events.Count)
                {
                    lifetimes.Add
                    (
                        new AllocationLife
                        {
                            Pointer = sorted_events[start_ix].Pointer,
                            Start = sorted_events[start_ix].Demarker,
                            End = NEVER,
                            AllocatingTrace = sorted_events[start_ix].CallingTrace
                        }
                    );
                    return lifetimes;
                }

                AllocationInfo start_event;
                AllocationInfo next_event;
                while (event_ix < sorted_events.Count)
                {
                    start_event = sorted_events[start_ix];
                    next_event = sorted_events[event_ix];

                    if (start_event.Pointer == next_event.Pointer)
                    {
                        start_ix += 2;
                        event_ix += 2;
                        if (excludeDeallocations) continue;
                        // If the next event is a deallocation, we have reached the end of the current lifetime
                        // Add the current lifetime to the list
                        lifetimes.Add
                        (
                            new AllocationLife
                            {
                                Pointer = start_event.Pointer,
                                Start = start_event.Demarker,
                                End = next_event.Demarker,
                                AllocatingTrace = start_event.CallingTrace,
                                DeallocatingTrace = next_event.CallingTrace
                            }
                        );
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
                                End = NEVER,
                                AllocatingTrace = start_event.CallingTrace,
                            }
                        );
                        start_ix++;
                        event_ix++;
                    }
                }

                if (start_ix < sorted_events.Count)
                {
                    start_event = sorted_events[start_ix];
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

                // Sort lifetime list by pointer
                lifetimes.Sort
                (
                    (
                        a,
                        b
                    ) => a.Pointer.ToInt64().CompareTo(b.Pointer.ToInt64())
                );
                return lifetimes;
            }

            public static List<AllocationInfo> GetEventsForPointer
            (
                IntPtr pointer
            )
            {
                return AllocationEventStack.FindAll((info => info.Pointer == pointer));
            }

            public static Demarker GetDemarker
            (
                string name
            )
            {
                return DemarkerStack.Find(demarker => demarker.Name == name);
            }

            public static void RegisterBoundary
            (
                string name
            )
            {
                var demarker = new Demarker
                {
                    Name = name,
                    Index = AllocationEventStack.Count
                };
                AllocationEventStack.Add
                (
                    CreateInfo
                    (
                        IntPtr.Zero,
                        Event.Boundary,
                        demarker
                    )
                );
                DemarkerStack.Add(demarker);
            }

            public static void RegisterAllocation
            (
                IntPtr pointer
            )
            {
                // Register pointer in map of all allocated pointers
                AllocatedPointers.Add((long)pointer);
                // Register the allocation event
                AllocationEventStack.Add
                (
                    CreateInfo(pointer, Event.Allocation)
                );

                // Ensure deallocation map has the pointer.
                DeallocationMap.TryAdd((long)pointer, 0);
                if (AllocationMap.TryAdd((long)pointer, 1)) return;

                // The allocation map already has the pointer from a previous use of the address.
                // Increment the allocation count.
                AllocationMap[(long)pointer]++;
            }

            public static void RegisterDeallocation
            (
                IntPtr pointer
            )
            {
                // Deregister pointer in map of all allocated pointers
                AllocatedPointers.Remove((long)pointer);
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
                    DeallocationMap[(long)pointer]++;
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

            private static AllocationInfo CreateInfo
            (
                IntPtr pointer,
                Event eventType,
                Demarker? demarker = null
            )
            {
                // TODO: begin Remove this -- Debug workaround
                var name = (demarker ?? new Demarker()).Name;
                if (name == "DisposeInitialActionSetters")
                {
                    var a = name;
                }

                // TODO: end
                return new AllocationInfo
                (
                    demarker ?? GetMostRecentDemarker(),
                    pointer,
                    eventType,
                    new StackTrace()
                );
            }

            private static Demarker GetMostRecentDemarker()
            {
                return DemarkerStack[^1];
            }

            // List of tuples of (IntPtr, AllocationCount, DeallocationCount)
            public static List<(long, int, int)> GetNotDeallocated()
            {
                var result = new List<(long, int, int)>();
                foreach (var elem in AllocationMap)
                {
                    var matched = DeallocationMap[elem.Key];
                    if (elem.Value == matched) continue;
                    result.Add((elem.Key, elem.Value, matched));
                }

                return result;
            }

            public static List<AllocationLife> GetNotDeallocatedLifeTimes()
            {
                var not_deallocated = GetNotDeallocated();
                var to_pointers = not_deallocated.Select(elem => elem.Item1).ToList();
                return GetLifeTimes(true, to_pointers.ToArray());
            }
        }
    }
}