using System;
using System.Text;
using NUnit.Framework;

namespace ReflectionObjectScales.Tests
{
    public class StringTests
    {
        [Test]
        public void TestConstString()
        {
            var str = "Const_str";
            Assert.AreEqual(40, str.GetExclusiveSize());
        }

        [Test]
        public void TestGeneratedString()
        {
            var stringBuilder = new StringBuilder("V:");
            stringBuilder.Append(new Random().Next(0, 9));

            Assert.AreEqual(28, stringBuilder.ToString().GetExclusiveSize());
        }

        [Test]
        public void TestMultibyteString()
        {
            var str = "𤭢𐐷";
            Assert.AreEqual(30, str.GetExclusiveSize());
        }
    }
}