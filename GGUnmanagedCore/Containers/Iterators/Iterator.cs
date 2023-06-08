using UnmanagedAPI;

namespace UnmanagedCore.Containers.Iterators
{
    public static partial class Iterator
    {
        // Provide an iterator for a generic Slice
        public static ArrayIterator<TUnmanaged> Get<TUnmanaged>
        (
            Allocation.Slice<TUnmanaged> head
        )
            where TUnmanaged : unmanaged
        {
            return new ArrayIterator<TUnmanaged>(head);
        }

        // Provide iterator for PointerList<T>
        public static ArrayIterator<TUnmanaged> Get<TUnmanaged>
        (
            PointerList<TUnmanaged> list,
            int? start = null
        )
            where TUnmanaged : unmanaged
        {
            int start_index = start ?? 0;
            return new ArrayIterator<TUnmanaged>(list.GetSlice(start_index));
        }
        
        // Provide iterator for PointerArray<T>
        public static ArrayIterator<TUnmanaged> Get<TUnmanaged>
        (
            PointerArray<TUnmanaged> array,
            int? start = null
        )
            where TUnmanaged : unmanaged
        {
            int start_index = start ?? 0;
            return new ArrayIterator<TUnmanaged>(array.GetSlice(start_index));
        }
        
        // Provide iterator for LinkedList<T>
        public static NodeIterator<TUnmanaged> Get<TUnmanaged>
        (
            LinkedList<TUnmanaged> list
        )
            where TUnmanaged : unmanaged
        {
            return new NodeIterator<TUnmanaged>(list.Head);
        }
    }
}