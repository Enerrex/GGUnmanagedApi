using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GGUnmanagedApi.Core.Pointer.Tests
{
    [TestClass]
    public unsafe class TestAllocationOwner
    {
        [TestMethod]
        public void TestNonNullOwner()
        {
            var allocated = Allocation.AllocateNew<int>();
            Assert.IsTrue((IntPtr) allocated.Pointer != IntPtr.Zero, "Allocated memory location should not be null.");
        }

        [TestMethod]
        public void TestSetPointerLocation()
        {
            var allocated = Allocation.AllocateNew<int>(2);

            Assert.IsTrue((IntPtr) allocated.Pointer != IntPtr.Zero, "Allocated memory location should not be null.");
            
            *allocated.Pointer = 1337;
            *(allocated.Pointer + 1) = 1338;
            
            Assert.AreEqual(*allocated.Pointer, 1337);
            Assert.AreEqual(allocated.Pointer[0], 1337);
            Assert.AreEqual(allocated[0], 1337); 
            
            Assert.AreEqual(*(allocated.Pointer + 1), 1338);
            Assert.AreEqual(allocated.Pointer[1], 1338);
            Assert.AreEqual(allocated[1], 1338);
        }

        struct TestObject
        {
            public int A;
            public int B;
        }

        [TestMethod]
        public void TestInitialize()
        {
            var allocated_int = Allocation.Initialize<int>(2);
            Assert.AreEqual(*allocated_int.Pointer, 0);
            Assert.AreEqual(allocated_int.Pointer[1], 0);

            AllocationOwner<TestObject> allocated = Allocation.Initialize<TestObject>
            (
                5,
                new TestObject
                {
                    A = 4,
                    B = 5
                }
            );
            
            // Verify that all 5 slots of allocated memory are initialized to the same value.
            for (var i = 0; i < 5; i++)
            {
                Assert.AreEqual((allocated.Pointer + i)->A, 4);
                Assert.AreEqual((allocated.Pointer + i)->B, 5);
            }
        }
    }
}