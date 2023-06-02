using System;
using UnmanagedAPI;

namespace Tests;

[TestClass]
public unsafe class TestAllocationOwner
{
    [TestMethod]
    public void TestNonNullOwner()
    {
        var allocated = Allocation.Create<int>();
        Assert.IsTrue((IntPtr)allocated.Pointer != IntPtr.Zero, "Allocated memory location should not be null.");
    }

    [TestMethod]
    public void TestSetPointerLocation()
    {
        var allocated = Allocation.Create<int>(2);

        Assert.IsTrue((IntPtr)allocated.Pointer != IntPtr.Zero, "Allocated memory location should not be null.");

        *allocated.Pointer = 1337;
        *(allocated.Pointer + 1) = 1338;

        Assert.AreEqual(*allocated.Pointer, 1337);
        Assert.AreEqual(allocated.Pointer[0], 1337);

        Assert.AreEqual(*(allocated.Pointer + 1), 1338);
        Assert.AreEqual(allocated.Pointer[1], 1338);
    }

    [TestMethod]
    public void TestInitializeInt()
    {
        var expected_int = 0;

        var allocated_int = Allocation.Initialize<int>(2);
        using (allocated_int)
        {
            Assert.AreEqual(expected_int, *allocated_int.Pointer);
            Assert.AreEqual(expected_int, allocated_int.Pointer[1]);
        }

        allocated_int = Allocation.Initialize(expected_int, 2);
        using (allocated_int)
        {
            Assert.AreEqual(expected_int, *allocated_int.Pointer);
            Assert.AreEqual(expected_int, allocated_int.Pointer[1]);
        }
    }

    [TestMethod]
    public void TestInitializeStruct()
    {
        var expected_struct = new TestObject { A = 0, B = 0 };

        var allocated_struct = Allocation.Initialize<TestObject>(2);
        using (allocated_struct)
        {
            Assert.AreEqual(expected_struct.A, allocated_struct.Pointer[0].A);
            Assert.AreEqual(expected_struct.B, allocated_struct.Pointer[0].B);

            Assert.AreEqual(expected_struct.A, allocated_struct.Pointer[1].A);
            Assert.AreEqual(expected_struct.B, allocated_struct.Pointer[1].B);
        }

        allocated_struct = Allocation.Initialize(expected_struct, 2);
        using (allocated_struct)
        {
            Assert.AreEqual(expected_struct.A, allocated_struct.Pointer[0].A);
            Assert.AreEqual(expected_struct.B, allocated_struct.Pointer[0].B);

            Assert.AreEqual(expected_struct.A, allocated_struct.Pointer[1].A);
            Assert.AreEqual(expected_struct.B, allocated_struct.Pointer[1].B);
        }
    }

    private struct TestObject
    {
        public int A;
        public int B;
    }
}