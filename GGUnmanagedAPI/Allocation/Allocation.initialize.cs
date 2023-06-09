﻿namespace UnmanagedAPI
{
    public static unsafe partial class Allocation
    {
        public static Owner<TUnmanaged> Initialize<TUnmanaged>
        (
            TUnmanaged value,
            int length
        ) where TUnmanaged : unmanaged
        {
            var allocated = Create<TUnmanaged>(length);

            for (var i = 0; i < length; i++) *(allocated.Pointer + i) = value;
            return allocated;
        }
        
        public static Owner<TUnmanaged> Initialize<TUnmanaged>
        (
            int length
        ) where TUnmanaged : unmanaged
        {
            var allocated = Create<TUnmanaged>(length);

            for (var i = 0; i < length; i++) *(allocated.Pointer + i) = new TUnmanaged();
            return allocated;
        }
        
        public static Owner<TUnmanaged> Initialize<TUnmanaged>
        (
            in TUnmanaged value
        ) where TUnmanaged : unmanaged
        {
            var allocated = Create<TUnmanaged>();

            *allocated.Pointer = value;
            return allocated;
        }

        /// <summary>
        /// Initialize a new reference with the values of another reference.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="allocationLength"></param>
        /// <typeparam name="TUnmanaged"></typeparam>
        public static void Initialize<TUnmanaged>
        (
            Reference<TUnmanaged> reference,
            int allocationLength=0
        ) where TUnmanaged : unmanaged
        {
            var allocated = Create<TUnmanaged>(allocationLength);
            for (var ix = 0; ix < allocationLength; ix++)
            {
                *(allocated.ToPointer() + ix) = *reference.ToPointer(ix);
            }
        }
    }
}