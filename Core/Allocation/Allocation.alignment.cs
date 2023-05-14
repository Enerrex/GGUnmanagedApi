namespace Core
{
    public static unsafe partial class Allocation
    {
        /// <summary>
        ///     Gets the alignment of a given type.
        ///     This is the number of bytes that must be added to the size of the type to make it aligned.
        /// </summary>
        /// <typeparam name="T">Unmanaged type T</typeparam>
        private static int AlignOf<T>() where T : unmanaged
        {
            return SizeOf<AlignOfHelper<T>>() - SizeOf<T>();
        }

        private static int SizeOf<T>() where T : unmanaged
        {
            return sizeof(T);
        }

        private struct AlignOfHelper<T> where T : unmanaged
        {
#pragma warning disable CS0649
            public byte Padding;
            public T Data;
#pragma warning restore CS0649
        }
    }
}