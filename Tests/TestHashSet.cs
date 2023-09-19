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
        public bool AreEqual
        (
            int a,
            int b
        )
        {
            return a == b;
        }
    }
    
    [TestMethod]
    public void TestTrivial()
    {
        var set = new HashSet<int, IntHasher>(33);
        
        // Add numbers 0-31
        for (int i = 0; i < 32; i++)
        {
            set.Add(i);
        }
        set.Add(0);
        
        // Check that all numbers are in the set
        for (int i = 0; i < 32; i++)
        {
            Assert.IsTrue(set.HasKey(i));
        }
    }
}