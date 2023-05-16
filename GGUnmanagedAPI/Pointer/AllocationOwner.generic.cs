using System;

namespace UnmanagedAPI.Pointer
{
    public readonly unsafe struct AllocationOwner<TUnmanaged> : IDisposable
        where TUnmanaged : unmanaged
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public TUnmanaged* Pointer { get; }

        public AllocationOwner
        (
            TUnmanaged* pointer
        )
        {
            Pointer = pointer;
        }

        public TUnmanaged this
        [
            int index
        ]
        {
            get => *(Pointer + index);
            set => *(Pointer + index) = value;
        }

        public AllocationReference<TUnmanaged> ToReference()
        {
            return new AllocationReference<TUnmanaged>(this);
        }

        public void Dispose()
        {
            Allocation.Delete(this);
        }

        #region OPERATORS

        public static implicit operator IntPtr
        (
            AllocationOwner<TUnmanaged> allocationOwner
        )
        {
            return (IntPtr)allocationOwner.Pointer;
        }

        public static implicit operator TUnmanaged*
        (
            AllocationOwner<TUnmanaged> pointer
        )
        {
            return pointer.Pointer;
        }

        public static implicit operator AllocationReference<TUnmanaged>
        (
            AllocationOwner<TUnmanaged> allocationOwner
        )
        {
            return allocationOwner.ToReference();
        }
        
        public static TUnmanaged* operator ~
        (
            AllocationOwner<TUnmanaged> allocationReference
        )
        {
            return allocationReference.Pointer;
        }

        #endregion
    }
}