using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GGUnmanagedApi.Core.Containers.Tests
{
    [TestClass]
    public class TestPointerArray
    {
        [TestMethod]
        public unsafe void TestTrivial()
        {
            var array = new PointerArray<int>();
            Assert.AreEqual(array.Length, 0);

            Assert.ThrowsException<IndexOutOfRangeException>(() => array[0]);
            Assert.ThrowsException<IndexOutOfRangeException>(() => array[0] = 1);
            
            array.Dispose();

            array = new PointerArray<int>(2);
            
            Assert.AreEqual(array.Length, 2);
            array[0] = 3;
            array[1] = 4;
            Assert.AreEqual(array[0], 3);
            Assert.AreEqual(array[1], 4);
            
            Assert.AreEqual(*array.AllocationReference.ToPointer(), 3);
            Assert.AreEqual(*array.AllocationReference.ToPointer(1), 4);
            Assert.AreEqual(*(array.AllocationReference.ToPointer() + 1), 4);
            
            Assert.ThrowsException<IndexOutOfRangeException>(() => array[2]);
            
            array.Dispose();
            Assert.AreEqual(array.Length, 0);
        }
        
        [TestMethod]
        public void TestNonTrivial()
        {
            var array = new PointerArray<TestObject>();
            Assert.AreEqual(array.Length, 0);

            Assert.ThrowsException<IndexOutOfRangeException>(() => array[0]);
            Assert.ThrowsException<IndexOutOfRangeException>(() => array[0] = new TestObject());
            
            array.Dispose();

            array = new PointerArray<TestObject>(2);
            
            Assert.AreEqual(array.Length, 2);
            array[0] = new TestObject
            {
                A = 3,
                B = 4
            };
            array[1] = new TestObject
            {
                A = 5,
                B = 6
            };
            Assert.AreEqual(array[0].A, 3);
            Assert.AreEqual(array[0].B, 4);
            Assert.AreEqual(array[1].A, 5);
            Assert.AreEqual(array[1].B, 6);
            
            Assert.ThrowsException<IndexOutOfRangeException>(() => array[2]);
            
            array.Dispose();
            Assert.AreEqual(array.Length, 0);
        }
    }
    
    struct TestObject
    {
        public int A;
        public int B;
    }
}