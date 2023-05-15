namespace UnmanagedAPI.Iterator
{
    public readonly struct MoveNextResult
    {
        public bool Success { get; }
        public int Index { get; }

        private MoveNextResult
        (
            bool success,
            int index
        )
        {
            Success = success;
            Index = index;
        }

        public static implicit operator MoveNextResult
        (
            (bool, int) tuple
        )
        {
            return new MoveNextResult(tuple.Item1, tuple.Item2);
        }

        public static implicit operator (bool, int)
        (
            MoveNextResult result
        )
        {
            return (result.Success, result.Index);
        }

        // Destructuring
        public void Deconstruct
        (
            out bool success,
            out int index
        )
        {
            success = Success;
            index = Index;
        }
    }
}