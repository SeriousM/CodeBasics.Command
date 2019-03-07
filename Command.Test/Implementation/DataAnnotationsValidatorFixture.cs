using System;
using System.Collections.Generic;
using CodeBasics.Command.Implementation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;

namespace CodeBasics.Command.Test.Implementation
{
  [TestClass]
  public class DataAnnotationsValidatorFixture
  {
    [TestMethod]
    public void LogErrorWhenValueIsRefTypeAndNull()
    {
      var logger = new Mock<ILogger<DataAnnotationsValidator<List<int>>>>();

      logger.Setup(
        l => l.Log(
          LogLevel.Error,
          0,
          new FormattedLogValues($"The value of type '{typeof(List<int>)}' cannot be null.", null),
          null,
          It.IsAny<Func<object, Exception, string>>())).Verifiable();
      var validator = new DataAnnotationsValidator<List<int>>(logger.Object);

      validator.Validate(null);

      logger.Verify();
    }

    [TestMethod]
    public void LogErrorWhenDataAnnotationOnValueIsNotValid()
    {
      var logger = new Mock<ILogger<DataAnnotationsValidator<SampleModel>>>();
      logger.Setup(
        l => l.Log(
          LogLevel.Error,
          0,
          It.IsAny<object>(),
          null,
          It.IsAny<Func<object, Exception, string>>())).Verifiable();
      var validator = new DataAnnotationsValidator<SampleModel>(logger.Object);

      validator.Validate(
        new SampleModel
        {
          Name = string.Empty
        });

      logger.Verify();
    }

    [TestMethod]
    public void LogDebugWhenDataAnnotationOnValueIsValid()
    {
      var logger = new Mock<ILogger<DataAnnotationsValidator<SampleModel>>>();
      logger.Setup(
        l => l.Log(
          LogLevel.Debug,
          0,
          It.IsAny<object>(),
          null,
          It.IsAny<Func<object, Exception, string>>())).Verifiable();
      var validator = new DataAnnotationsValidator<SampleModel>(logger.Object);

      validator.Validate(
        new SampleModel
        {
          Name = "Bob"
        });

      logger.Verify();
    }

    [TestMethod]
    public void ReturnFalseWhenDataAnnotationOnValueIsNotValid()
    {
      var logger = new Mock<ILogger<DataAnnotationsValidator<SampleModel>>>();
      var validator = new DataAnnotationsValidator<SampleModel>(logger.Object);

      var valid = validator.Validate(
        new SampleModel
        {
          Name = string.Empty
        });

      valid.IsValid.ShouldBeFalse();
    }

    [TestMethod]
    public void ReturnFalseWhenDataAnnotationOnValueIsValidButOnValidateReturnFalse()
    {
      var sampleModel = new SampleModel
      {
        Name = "correct Value"
      };
      var logger = new Mock<ILogger<DataAnnotationsValidator<SampleModel>>>();
      var validator = Mock.CreateInstanceOf<DataAnnotationsValidator<SampleModel>>(
        m => m.Setup(v => v.OnValidate(sampleModel)).Returns(new ValidationStatus(false)),
        logger.Object);

      var valid = validator.Validate(
        sampleModel);

      valid.IsValid.ShouldBeFalse();
    }

    [TestMethod]
    public void ReturnFalseWhenValueIsRefTypeAndNull()
    {
      var logger = new Mock<ILogger<DataAnnotationsValidator<List<int>>>>();
      var validator = new DataAnnotationsValidator<List<int>>(logger.Object);

      var valid = validator.Validate(null);

      valid.IsValid.ShouldBeFalse();
    }

    [TestMethod]
    public void ReturnTrueWhenDataAnnotationOnValueIsValid()
    {
      var logger = new Mock<ILogger<DataAnnotationsValidator<SampleModel>>>();
      var validator = new DataAnnotationsValidator<SampleModel>(logger.Object);

      var valid = validator.Validate(
        new SampleModel
        {
          Name = "correct value"
        });

      valid.IsValid.ShouldBeTrue();
    }

    [TestMethod]
    public void ReturnTrueWhenValueIsByRefAndNotNull()
    {
      var validator = new DataAnnotationsValidator<List<int>>(Moq.Mock.Of<ILogger<DataAnnotationsValidator<List<int>>>>());

      var valid = validator.Validate(new List<int>());

      valid.IsValid.ShouldBeTrue();
    }

    [TestMethod]
    public void ReturnTrueWhenValueIsPrimitiveType()
    {
      var validator = new DataAnnotationsValidator<decimal>(Moq.Mock.Of<ILogger<DataAnnotationsValidator<decimal>>>());

      var valid = validator.Validate(0);

      valid.IsValid.ShouldBeTrue();
    }

    [TestMethod]
    public void ReturnTrueWhenValueIsStringTypeWithValue()
    {
      var validator = new DataAnnotationsValidator<string>(Moq.Mock.Of<ILogger<DataAnnotationsValidator<string>>>());

      var valid = validator.Validate("");

      valid.IsValid.ShouldBeTrue();
    }

    [TestMethod]
    public void ReturnFalseWhenValueIsStringTypeWithoutValue()
    {
      var validator = new DataAnnotationsValidator<string>(Moq.Mock.Of<ILogger<DataAnnotationsValidator<string>>>());

      var valid = validator.Validate(null);

      valid.IsValid.ShouldBeFalse();
    }

    [TestMethod]
    public void ReturnTrueWhenValueIsNullableTypeWithValue()
    {
      var validator = new DataAnnotationsValidator<int?>(Moq.Mock.Of<ILogger<DataAnnotationsValidator<int?>>>());

      var valid = validator.Validate(1);

      valid.IsValid.ShouldBeTrue();
    }

    [TestMethod]
    public void ReturnFalseWhenValueIsNullableTypeWithoutValue()
    {
      var validator = new DataAnnotationsValidator<int?>(Moq.Mock.Of<ILogger<DataAnnotationsValidator<int?>>>());

      var valid = validator.Validate(null);

      valid.IsValid.ShouldBeFalse();
    }
  }
}
