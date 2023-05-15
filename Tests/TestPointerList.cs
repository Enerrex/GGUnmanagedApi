using System;
using Core.Containers;

namespace Tests;

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

    [TestMethod]
    public void TestExpand()
    {
        var list = new PointerList<int>(10);
        Assert.AreEqual(10, list.Capacity);
        Assert.AreEqual(0, list.Count);

        for (var i = 0; i < 10; i++) list.Add(i * 10);

        Assert.AreEqual(10, list.Capacity);
        Assert.AreEqual(10, list.Count);

        // Check that values are correct
        for (var i = 0; i < 10; i++) Assert.AreEqual(i * 10, list[i]);

        list.Add(101);
        Assert.AreEqual(20, list.Capacity);
        Assert.AreEqual(11, list.Count);
    }

    [TestMethod]
    public void TestAddListToList()
    {
        var list1 = new PointerList<int>(10);
        var list2 = new PointerList<int>(10);
        for (var i = 0; i < 10; i++)
        {
            list1.Add(i * 10);
            list2.Add(i * 100);
        }

        // test that the values are correct
        for (var i = 0; i < 10; i++)
        {
            Assert.AreEqual(i * 10, list1[i]);
            Assert.AreEqual(i * 100, list2[i]);
        }

        // When adding a list to another list, if the capacity is exceeded,
        // the capacity will be doubled and have the Count of the other list added to it
        // e.g. if list1 has a capacity of 10 and list2 has a capacity of 10, and we add list2 to list1,
        // the capacity of list1 will be 10 * 2 + 10 = 30
        list1.Add(list2);
        Assert.AreEqual(30, list1.Capacity);
        Assert.AreEqual(20, list1.Count);
        Assert.AreEqual(10, list2.Capacity);
        Assert.AreEqual(10, list2.Count);

        for (var i = 0; i < 10; i++)
        {
            Assert.AreEqual(i * 10, list1[i]);
            Assert.AreEqual(i * 100, list1[i + 10]);
        }
    }

    [TestMethod]
    public void TestAddListToListWithEnoughSpace()
    {
        var list1 = new PointerList<int>(20);
        var list2 = new PointerList<int>(10);

        for (var i = 0; i < 10; i++)
        {
            list1.Add(i * 10);
            list2.Add(i * 100);
        }

        // test that the values are correct
        for (var i = 0; i < 10; i++)
        {
            Assert.AreEqual(i * 10, list1[i]);
            Assert.AreEqual(i * 100, list2[i]);
        }

        // Add list2 to list1
        // Since list1 has a capacity of 20 and list2 has a count of 10, the capacity of list1 will not be expanded
        list1.Add(list2);
        Assert.AreEqual(20, list1.Capacity);
    }
}