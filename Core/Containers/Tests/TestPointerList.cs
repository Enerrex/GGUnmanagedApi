using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GGUnmanagedApi.Core.Containers.Tests
{
    [TestClass]
    public class TestPointerList
    {
        [TestMethod]
        public void TestTrivial()
        {
            var list = new PointerList<int>();
            Assert.AreEqual(list.Capacity, 0);
            Assert.AreEqual(list.Count, 0);

            Assert.ThrowsException<IndexOutOfRangeException>(() => list[0]);
            Assert.ThrowsException<IndexOutOfRangeException>(() => list[0] = 1);
            
            list.Dispose();
            
            list = new PointerList<int>(2);
            Assert.AreEqual(2, list.Capacity);
            Assert.AreEqual(0, list.Count);
            
            list.Add(3);
            list.Add(4);
            Assert.AreEqual(list[0], 3);
            Assert.AreEqual(list[1], 4);
            Assert.ThrowsException<IndexOutOfRangeException>(() => list[2]);
        }
    }
}