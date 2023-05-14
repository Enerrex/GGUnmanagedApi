using System;
using Core.Containers;

namespace Tests
{
    [TestClass]
    public unsafe class TestLinkedList
    {
        [TestMethod]
        public void TestTrivial()
        {
            LinkedNode<int> node = new LinkedNode<int>();
        }
    }
}