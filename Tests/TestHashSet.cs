namespace Tests;
using UnmanagedCore.Containers.Hashing;

[TestClass]
public class TestHashSet
{
    [TestMethod]
    public void TestTrivial()
    {
        var set = new HashSet<int>();

        Assert.AreEqual(0, set.Count);
        Assert.IsTrue(set.IsEmpty);
    }
}