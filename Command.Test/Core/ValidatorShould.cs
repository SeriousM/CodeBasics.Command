using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Command.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Command.Test.Core
{
  [TestFixture]
  public class ValidatorShould
  {
    [Test]
    public void LogErrorWhenValueIsRefTypeAndNull()
    {
      var logger = new Mock<ILogger<Validator<List<int>>>>();

      logger.Setup(
        l => l.Log(
          LogLevel.Error,
          0,
          new FormattedLogValues("The value can not be null.", null),
          null,
          It.IsAny<Func<object, Exception, string>>())).Verifiable();
      var validator = new Validator<List<int>>(logger.Object);

      validator.Validate(null);

      logger.Verify();
    }

    [Test]
    public void LogInformationWhenDataAnnotationOnValueIsNotValid()
    {
      ValidationAttribute attribute = new RequiredAttribute();
      var propertyInfo = attribute.GetType().GetProperty(
        "ErrorMessageString",
        BindingFlags.Instance | BindingFlags.NonPublic);
      var expectedMessage = propertyInfo.GetValue(attribute) as string;

      var logger = new Mock<ILogger<Validator<SampleModel>>>();
      logger.Setup(
        l => l.Log(
          LogLevel.Information,
          0,
          new FormattedLogValues(string.Format(expectedMessage, "Name"), null),
          null,
          It.IsAny<Func<object, Exception, string>>())).Verifiable();
      var validator = new Validator<SampleModel>(logger.Object);

      validator.Validate(
        new SampleModel
        {
          Name = string.Empty
        });

      logger.Verify();
    }

    [Test]
    public void ReturnFalseWhenDataAnnotationOnValueIsNotValid()
    {
      var logger = new Mock<ILogger<Validator<SampleModel>>>();
      var validator = new Validator<SampleModel>(logger.Object);

      var valid = validator.Validate(
        new SampleModel
        {
          Name = string.Empty
        });

      valid.ShouldBeFalse();
    }

    [Test]
    public void ReturnFalseWhenDataAnnotationOnValueIsValidButOnValidateReturnFalse()
    {
      var sampleModel = new SampleModel
      {
        Name = "correct Value"
      };
      var logger = new Mock<ILogger<Validator<SampleModel>>>();
      var validator = Mock.CreateInstanceOf<Validator<SampleModel>>(
        m => m.Setup(v => v.OnValidate(sampleModel)).Returns(false),
        logger.Object);

      var valid = validator.Validate(
        sampleModel);

      valid.ShouldBeFalse();
    }

    [Test]
    public void ReturnFalseWhenValueIsRefTypeAndNull()
    {
      var logger = new Mock<ILogger<Validator<List<int>>>>();
      var validator = new Validator<List<int>>(logger.Object);

      var valid = validator.Validate(null);

      valid.ShouldBeFalse();
    }

    [Test]
    public void ReturnTrueWhenDataAnnotationOnValueIsValid()
    {
      var logger = new Mock<ILogger<Validator<SampleModel>>>();
      var validator = new Validator<SampleModel>(logger.Object);

      var valid = validator.Validate(
        new SampleModel
        {
          Name = "correct value"
        });

      valid.ShouldBeTrue();
    }

    [Test]
    public void ReturnTrueWhenValueIsByRefAndNotNull()
    {
      var validator = new Validator<List<int>>(It.IsAny<ILogger<Validator<List<int>>>>());

      var valid = validator.Validate(new List<int>());

      valid.ShouldBeTrue();
    }

    [Test]
    public void ReturnTrueWhenValueIsPrimitiveType()
    {
      var validator = new Validator<string>(It.IsAny<ILogger<Validator<string>>>());

      var valid = validator.Validate(It.IsAny<string>());

      valid.ShouldBeTrue();
    }

    [Test]
    public void ReturnTrueWhenValueNullableType()
    {
      var validator = new Validator<int?>(It.IsAny<ILogger<Validator<int?>>>());

      var valid = validator.Validate(null);

      valid.ShouldBeTrue();
    }
  }
}
