using System;
using NUnit.Framework;
using ReflectionObjectScales.Tests.Samples;
using ValueType = ReflectionObjectScales.Tests.Samples.ValueType;

namespace ReflectionObjectScales.Tests
{
    /// <summary>
    ///     See <see cref="ReflectionObjectScales.Tests.Samples" /> namespace for used samples.
    /// </summary>
    public class SamplesTests
    {
        [Test]
        public void TestEmptyObject()
        {
            var obj = new Empty();
            Assert.AreEqual(Empty.DebuggerSize, obj.GetExclusiveSize());
        }

        [Test]
        public void TestSingleInt()
        {
            var obj = new SingleInt();
            Assert.AreEqual(SingleInt.DebuggerSize, obj.GetExclusiveSize());
        }

        [Test]
        public void TestDoubleLong()
        {
            var obj = new DoubleLong {L1 = 1, L2 = 2};
            Assert.AreEqual(DoubleLong.DebuggerSize, obj.GetExclusiveSize());
        }

        [Test]
        public void TestInherited()
        {
            var obj = new Inherited {L2 = 2};
            Assert.AreEqual(Inherited.DebuggerSize, obj.GetExclusiveSize());
        }

        [Test]
        public void TestInheritedHierarchy()
        {
            var obj = new InheritedHierarchy {L2 = 2, B4 = 4};
            Assert.AreEqual(InheritedHierarchy.DebuggerSize, obj.GetExclusiveSize());
        }

        [Test]
        public void TestBigValueType()
        {
            var obj = new BigValueType {L1 = 1, L2 = 2, L3 = 3, L4 = 4};
            Assert.AreEqual(BigValueType.DebuggerSize, obj.GetExclusiveSize());
        }

        [Test]
        public void TestNested()
        {
            var nested = new Nested();
            var inside = new Nested.Inside {L1 = 1};
            Assert.AreEqual(Nested.DebuggerSize, nested.GetExclusiveSize());
            Assert.AreEqual(Nested.Inside.DebuggerSizeInside, inside.GetExclusiveSize());
        }

        [Test]
        public void TestPlainEnum()
        {
            var obj = PlainEnum.ONE;
            Assert.AreEqual(sizeof(int), obj.GetExclusiveSize());
        }

        [Test]
        public void TestNonPlainEnum()
        {
            var obj = NonPlainEnum.TWO;
            Assert.AreEqual(sizeof(byte), obj.GetExclusiveSize());
        }

        [Test]
        public void TestOnlyConst()
        {
            var obj = new OnlyConst();
            Assert.AreEqual(OnlyConst.DebuggerSize, obj.GetExclusiveSize());
        }

        [Test]
        public void TestPacked()
        {
            var obj = new Packed {B = 0xbb, I1 = 1, I2 = 2, L1 = 3, L2 = 4};
            Assert.AreEqual(Packed.DebuggerSize, obj.GetExclusiveSize());
        }

        [Test]
        public void TestPadded()
        {
            var obj = new Padded {B = 1, L = 2};
            Assert.AreEqual(Padded.DebuggerSize, obj.GetExclusiveSize());
        }

        [Test]
        public void TestValueType()
        {
            var obj = new ValueType {B = 1, L = 2};
            Assert.AreEqual(ValueType.DebuggerSize, obj.GetExclusiveSize());
        }

        [Test]
        public void TestWithDateTime()
        {
            var obj = new WithDateTime {Dt = DateTime.Now};
            Assert.AreEqual(WithDateTime.DebuggerSize, obj.GetExclusiveSize());
        }

        [Test]
        public void TestWithEnums()
        {
            var obj = new WithEnums {PlainEnum = PlainEnum.ONE, NonPlainEnum = NonPlainEnum.TWO};
            Assert.AreEqual(WithEnums.DebuggerSize, obj.GetExclusiveSize());
        }

        [Test]
        public void TestWithRef()
        {
            var obj = new WithRef {Ref = new Empty()};
            Assert.AreEqual(WithRef.DebuggerSize, obj.GetExclusiveSize());
        }

        [Test]
        public void TestWithString()
        {
            var obj = new WithString {Str = "Some arbitrary string"};
            Assert.AreEqual(WithString.DebuggerSize, obj.GetExclusiveSize());
        }

        [Test]
        public void TestWithValueType()
        {
            var obj = new WithValueType {I = 1, L = 2, ValueType = new ValueType {B = 3, L = 4}};
            Assert.AreEqual(WithValueType.DebuggerSize, obj.GetExclusiveSize());
        }
    }
}