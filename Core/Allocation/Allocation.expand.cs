using System;

namespace GGUnmanagedApi.Core
{
    public static unsafe partial class Allocation
    {
        public static IntPtr Expand<T>(T* copiedArray, int length, int targetLength) where T : unmanaged
        {
            return Copy(copiedArray, length, targetLength);
        }
    }
}