﻿namespace GGUnmanagedApi
{
    public interface IUnmanagedIterator<TNode> where TNode : unmanaged
    {
        TNode Current { get; }
        MoveNextResult MoveNext();
    }
    
    public struct MoveNextResult
    {
        public bool Success { get; }
        public int Index { get; }
        
        public MoveNextResult(bool success, int index)
        {
            Success = success;
            Index = index;
        }
        
        public static implicit operator MoveNextResult((bool, int) tuple) => new MoveNextResult(tuple.Item1, tuple.Item2);
        
        public static implicit operator (bool, int)(MoveNextResult result) => (result.Success, result.Index);
        
        // Destructuring
        public void Deconstruct(out bool success, out int index)
        {
            success = Success;
            index = Index;
        }
    }
}