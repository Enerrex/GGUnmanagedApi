using System;
using UnmanagedAPI.Containers;

namespace Tests;
using UnmanagedCore.Containers.Hashing;

[TestClass]
public class TestHashSet
{
    internal struct IntHasher : IHashProvider<int>
    {
        public int GetHashCode(in int item)
        {
            return item;
        }
    }
    
    [TestMethod]
    public void TestTrivial()
    {
        var set = new HashSet<int, IntHasher>(10);
        
        set.Add(2);
        set.Add(3);
        set.Add(2);
    }
}