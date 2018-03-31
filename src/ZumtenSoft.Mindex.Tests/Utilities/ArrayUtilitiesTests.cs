using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZumtenSoft.Mindex.Utilities;

namespace ZumtenSoft.Mindex.Tests.Utilities
{
    [TestClass]
    public class ArrayUtilitiesTests
    {
        private static int[] TenValues = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
        private static int[] EmptyArray = ArrayUtilities<int>.EmptyArray;
        private static ArraySegment<int> EmptySegment = ArrayUtilities<int>.EmptySegment;
        private static ArraySegment<int> SegmentOneValue = new ArraySegment<int>(TenValues, 3, 1);
        private static ArraySegment<int> SegmentTwoValue = new ArraySegment<int>(TenValues, 7, 2);

        [TestMethod]
        public void TotalCount_WhenEmpty_ShouldReturnEmptyArray()
        {
            Assert.AreEqual(0, ArrayUtilities<int>.TotalCount(new ArraySegment<int>[0]));
            Assert.AreEqual(0, ArrayUtilities<int>.TotalCount(new int[0][]));
            Assert.AreEqual(0, ArrayUtilities<int>.TotalCount(new[] { EmptySegment }));
            Assert.AreEqual(0, ArrayUtilities<int>.TotalCount(new[] { EmptyArray }));
        }

        [TestMethod]
        public void TotalCount_WhenNotEmpty_ShouldSumTheCount()
        {
            Assert.AreEqual(1, ArrayUtilities<int>.TotalCount(new[] { SegmentOneValue }));
            Assert.AreEqual(10, ArrayUtilities<int>.TotalCount(new[] { TenValues }));
            Assert.AreEqual(3, ArrayUtilities<int>.TotalCount(new[] { SegmentOneValue, SegmentTwoValue }));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Flatten_WhenUninitializedArraySegment_ShouldThrowException()
        {
            ArrayUtilities<int>.Flatten(new[] {new ArraySegment<int>()});
        }
    }
}
