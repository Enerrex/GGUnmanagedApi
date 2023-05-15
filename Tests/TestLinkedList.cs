using System;
using Core.Containers;

namespace Tests;

[TestClass]
public unsafe class TestLinkedList
{
    [TestMethod]
    public void TestTrivial()
    {
        var list = new LinkedList<int>();

        Assert.AreEqual(0, list.Count);
        Assert.IsTrue(list.IsEmpty);

        Assert.AreEqual(IntPtr.Zero, list.Head.IntPtr);
        Assert.AreEqual(IntPtr.Zero, list.Tail.IntPtr);
    }

    [TestMethod]
    public void TestAddNode()
    {
        var list = new LinkedList<int>();

        Assert.AreEqual(IntPtr.Zero, list.Head.IntPtr);
        Assert.AreEqual(IntPtr.Zero, list.Tail.IntPtr);

        list.AddNode(1);
        list.AddNode(2);
        list.AddNode(3);

        Assert.AreNotEqual(IntPtr.Zero, list.Head.IntPtr);
        Assert.AreNotEqual(IntPtr.Zero, list.Tail.IntPtr);

        Assert.AreEqual(3, list.Count);
        Assert.IsFalse(list.IsEmpty);

        Assert.AreEqual(1, list.Head.Pointer->Value);
        Assert.AreEqual(2, list.Head.Pointer->Next.Pointer->Value);
        Assert.AreEqual(3, list.Tail.Pointer->Value);
    }
}