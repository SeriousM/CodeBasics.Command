using System;
using Command.Extensions;
using NUnit.Framework;
using Shouldly;

namespace Command.Test.Extensions
{
  [TestFixture]
  public class TypeExtensionsShould
  {
    [Test]
    public void ReturnTrueWhenCallIsPrimitiveForDateTimeType()
    {
      var stringType = DateTime.Now.GetType();
      TypeExtensions.IsPrimitive(stringType).ShouldBeTrue();
    }

    [Test]
    public void ReturnTrueWhenCallIsPrimitiveForDecimalType()
    {
      var stringType = 1.2m.GetType();
      TypeExtensions.IsPrimitive(stringType).ShouldBeTrue();
    }

    [Test]
    public void ReturnTrueWhenCallIsPrimitiveForDoubleType()
    {
      var stringType = 1.3d.GetType();
      TypeExtensions.IsPrimitive(stringType).ShouldBeTrue();
    }

    [Test]
    public void ReturnTrueWhenCallIsPrimitiveForGuidType()
    {
      var guidType = Guid.NewGuid().GetType();
      TypeExtensions.IsPrimitive(guidType).ShouldBeTrue();
    }

    [Test]
    public void ReturnTrueWhenCallIsPrimitiveForIntType()
    {
      var stringType = 1.GetType();
      TypeExtensions.IsPrimitive(stringType).ShouldBeTrue();
    }

    [Test]
    public void ReturnTrueWhenCallIsPrimitiveForStringType()
    {
      var stringType = "".GetType();
      TypeExtensions.IsPrimitive(stringType).ShouldBeTrue();
    }
  }
}
