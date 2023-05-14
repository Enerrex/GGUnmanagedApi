using System;

namespace Core.Pointer
{
    public readonly unsafe struct AllocationReference<TUnmanaged> where TUnmanaged : unmanaged
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public TUnmanaged* Pointer { get; }
        
        public bool IsNull => (IntPtr)Pointer == IntPtr.Zero;

        public AllocationReference
        (
            AllocationOwner<TUnmanaged> pointer
        )
        {
            Pointer = pointer;
        }

        public AllocationReference
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

        public TUnmanaged* ToPointer
        (
            int index = 0
        )
        {
            return Pointer + index;
        }

        #region OPERATORS

        public static implicit operator IntPtr
        (
            AllocationReference<TUnmanaged> allocationReference
        )
        {
            return (IntPtr) allocationReference.Pointer;
        }

        public static implicit operator TUnmanaged*
        (
            AllocationReference<TUnmanaged> pointer
        )
        {
            return pointer.Pointer;
        }

        // Operator to convert from TUnmanaged* to AllocationReference<TUnmanaged>
        public static implicit operator AllocationReference<TUnmanaged>
        (
            TUnmanaged* pointer
        )
        {
            return new AllocationReference<TUnmanaged>(pointer);
        }

        // Operator to convert from AllocationOwner<TUnmanaged> to AllocationReference<TUnmanaged>
        public static implicit operator AllocationReference<TUnmanaged>
        (
            AllocationOwner<TUnmanaged> pointer
        )
        {
            return new AllocationReference<TUnmanaged>(pointer);
        }

        #endregion
    }
}