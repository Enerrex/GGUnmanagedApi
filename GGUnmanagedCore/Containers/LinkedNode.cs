using System;
using UnmanagedAPI;
using UnmanagedAPI.Pointer;

namespace Core.Containers
{
    /// <summary>
    ///     Important usage note: this struct circumvents the AllocationOwner struct because of a CLR limitation
    ///     We can't have a struct that as a generic type that references itself (even though it's a pointer)
    ///     We can however have a struct that has a raw pointer to itself.
    /// </summary>
    /// <typeparam name="TUnmanaged"></typeparam>
    public unsafe struct LinkedNode<TUnmanaged> : IOwner<LinkedNode<TUnmanaged>>, IDisposable
        where TUnmanaged : unmanaged
    {
        public readonly TUnmanaged Value;

        public AllocationReference<LinkedNode<TUnmanaged>> Next => new AllocationReference<LinkedNode<TUnmanaged>>(_next);

        private PointerOwner _next;

        public LinkedNode
        (
            TUnmanaged value
        )
        {
            Value = value;
            _next = default;
        }

        public AllocationReference<LinkedNode<TUnmanaged>> SetNext
        (
            TUnmanaged value
        )
        {
            if (_next != null) _next->Dispose();
            var allocation = Allocation.Create
            (
                new LinkedNode<TUnmanaged>(value)
            );
            _next = allocation.Pointer;
            return allocation.ToReference();
        }
        
        public static implicit operator TUnmanaged
        (
            LinkedNode<TUnmanaged> node
        )
        {
            return node.Value;
        }

        public AllocationReference<LinkedNode<TUnmanaged>> ToReference()
        {
            return new AllocationReference<LinkedNode<TUnmanaged>>(_next);
        }

        public void Dispose()
        {
            if ((IntPtr) Next.Pointer == IntPtr.Zero) return;
            // These must happen in this order
            //     Dispose of the next LinkedNode (which will dispose of the next LinkedNode, etc.)
            _next->Dispose();
            //     Dispose of our own allocation for the next node
            //     This is bad! We shouldn't ever do this anywhere else
            Allocation.Delete(this);
        }

        /// <summary>
        /// Copy Strategy for LinkedNode
        ///     Node: Copies the node itself, meaning we copy that chain of nodes (this is the default)
        ///         example scenario, give this node, and a node A being added as the next node:
        ///         this -> null
        ///         A -> B -> G -> null
        ///         setNext(A)
        ///         this -> A -> B -> G -> null
        ///     Value
        ///         Copy only the value of the node, do not copy the chain of nodes
        ///     Values: Copies the values of the nodes, but not the nodes themselves
        ///         A1 -> B1 -> G1 -> null
        ///         setNext(A)
        ///         this -> A2 -> B2 -> G2 -> null
        /// </summary>
        public enum LinkedNodeCopyStrategy
        {
            Node, // Inserts the nodes into the new list
            Value, // Copies the value of the node (the value it contains), but not the node itself
            Values, // Copies the values of the node and all subsequent nodes
        }
    }
}