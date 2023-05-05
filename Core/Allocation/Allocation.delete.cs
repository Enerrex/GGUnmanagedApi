using System;
using System.Runtime.InteropServices;

namespace GGUnmanagedApi.Core
{
    public static unsafe partial class Allocation
    {
        private static void Delete<T>
        (
            Owner<T> ownerPointer
        ) where T : unmanaged
        {
            if (ownerPointer == IntPtr.Zero)
            {
                throw new ArgumentException("Cannot free a zero memory pointer.", "ownerPointer");
            }

            Marshal.FreeHGlobal(ownerPointer);
        }
    }
}