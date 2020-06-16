using NUnit.Framework;
using ReflectionObjectScales.Tests.Samples;

namespace ReflectionObjectScales.Tests
{
    public class ArrayTests
    {
        [Test]
        public void TestIntArray()
        {
            var ints = new[] {0xCA, 0xFE, 0xBA, 0xBE, 0, 0, 0, 0, 0, 0};
            Assert.AreEqual(64, ints.GetExclusiveSize());
        }

        [Test]
        public void TestLongArray()
        {
            var longs = new long[] {0xCA, 0xFE, 0xBA, 0xBE, 0, 0, 0, 0, 0, 0};
            Assert.AreEqual(104, longs.GetExclusiveSize());
        }

        [Test]
        public void TestStructsArray()
        {
            var valueTypes = new ValueType[10];
            Assert.AreEqual(184, valueTypes.GetExclusiveSize());
        }
    }
}