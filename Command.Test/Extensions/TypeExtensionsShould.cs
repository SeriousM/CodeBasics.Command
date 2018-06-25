namespace Command.Test.Extensions
{
    using System;

    using Command.Extensions;

    using NUnit.Framework;

    using Shouldly;

    [TestFixture]
    public class TypeExtensionsShould
    {
        [Test]
        public void ReturnTrueWhenCallIsPrimitiveForDateTimeType()
        {
            Type stringType = DateTime.Now.GetType();
            stringType.IsPrimitive().ShouldBeTrue();
        }

        [Test]
        public void ReturnTrueWhenCallIsPrimitiveForDecimalType()
        {
            Type stringType = 1.2m.GetType();
            stringType.IsPrimitive().ShouldBeTrue();
        }

        [Test]
        public void ReturnTrueWhenCallIsPrimitiveForDoubleType()
        {
            Type stringType = 1.3d.GetType();
            stringType.IsPrimitive().ShouldBeTrue();
        }

        [Test]
        public void ReturnTrueWhenCallIsPrimitiveForGuidType()
        {
            Type guidType = Guid.NewGuid().GetType();
            guidType.IsPrimitive().ShouldBeTrue();
        }

        [Test]
        public void ReturnTrueWhenCallIsPrimitiveForIntType()
        {
            Type stringType = 1.GetType();
            stringType.IsPrimitive().ShouldBeTrue();
        }

        [Test]
        public void ReturnTrueWhenCallIsPrimitiveForStringType()
        {
            Type stringType = "".GetType();
            stringType.IsPrimitive().ShouldBeTrue();
        }
    }
}