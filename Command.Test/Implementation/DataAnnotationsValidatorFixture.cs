using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CodeBasics.Command.Implementation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
    public async Task LogErrorWhenValueIsRefTypeAndNull()
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

      await validator.ValidateAsync(null);

      logger.Verify();
    }

    [TestMethod]
    public async Task LogErrorWhenDataAnnotationOnValueIsNotValid()
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

      await validator.ValidateAsync(
        new SampleModel
        {
          Name = string.Empty
        });

      logger.Verify();
    }

    [TestMethod]
    public async Task LogDebugWhenDataAnnotationOnValueIsValid()
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

      await validator.ValidateAsync(
        new SampleModel
        {
          Name = "Bob"
        });

      logger.Verify();
    }

    [TestMethod]
    public async Task ReturnFalseWhenDataAnnotationOnValueIsNotValid()
    {
      var logger = new Mock<ILogger<DataAnnotationsValidator<SampleModel>>>();
      var validator = new DataAnnotationsValidator<SampleModel>(logger.Object);

      var valid = await validator.ValidateAsync(
        new SampleModel
        {
          Name = string.Empty
        });

      valid.IsValid.ShouldBeFalse();
    }

    [TestMethod]
    public async Task ReturnFalseWhenDataAnnotationOnValueIsValidButOnValidateReturnFalse()
    {
      var sampleModel = new SampleModel
      {
        Name = "correct Value"
      };
      var logger = new Mock<ILogger<DataAnnotationsValidator<SampleModel>>>();
      var validator = Mock.CreateInstanceOf<DataAnnotationsValidator<SampleModel>>(
        m => m.Setup(v => v.OnValidate(sampleModel)).Returns(new ValidationStatus(false)),
        logger.Object);

      var valid = await validator.ValidateAsync(
        sampleModel);

      valid.IsValid.ShouldBeFalse();
    }

    [TestMethod]
    public async Task ReturnFalseWhenValueIsRefTypeAndNull()
    {
      var logger = new Mock<ILogger<DataAnnotationsValidator<List<int>>>>();
      var validator = new DataAnnotationsValidator<List<int>>(logger.Object);

      var valid = await validator.ValidateAsync(null);

      valid.IsValid.ShouldBeFalse();
    }

    [TestMethod]
    public async Task ReturnTrueWhenDataAnnotationOnValueIsValid()
    {
      var logger = new Mock<ILogger<DataAnnotationsValidator<SampleModel>>>();
      var validator = new DataAnnotationsValidator<SampleModel>(logger.Object);

      var valid = await validator.ValidateAsync(
        new SampleModel
        {
          Name = "correct value"
        });

      valid.IsValid.ShouldBeTrue();
    }

    [TestMethod]
    public async Task ReturnTrueWhenValueIsByRefAndNotNull()
    {
      var validator = getValidator<List<int>>();

      var valid = await validator.ValidateAsync(new List<int>());

      valid.IsValid.ShouldBeTrue();
    }

    [TestMethod]
    public async Task ReturnTrueWhenValueIsPrimitiveType()
    {
      var validator = getValidator<decimal>();

      var valid = await validator.ValidateAsync(0);

      valid.IsValid.ShouldBeTrue();
    }

    [TestMethod]
    public async Task ReturnTrueWhenValueIsStringTypeWithValue()
    {
      var validator = getValidator<string>();

      var valid = await validator.ValidateAsync("");

      valid.IsValid.ShouldBeTrue();
    }

    [TestMethod]
    public async Task ReturnFalseWhenValueIsStringTypeWithoutValue()
    {
      var validator = getValidator<string>();

      var valid = await validator.ValidateAsync(null);

      valid.IsValid.ShouldBeFalse();
    }

    [TestMethod]
    public async Task ReturnTrueWhenValueIsNullableTypeWithValue()
    {
      var validator = getValidator<int?>();

      var valid = await validator.ValidateAsync(1);

      valid.IsValid.ShouldBeTrue();
    }

    [TestMethod]
    public async Task ReturnFalseWhenValueIsNullableTypeWithoutValue()
    {
      var validator = getValidator<int?>();

      var valid = await validator.ValidateAsync(null);

      valid.IsValid.ShouldBeFalse();
    }

    [TestMethod]
    public async Task Validate_array_with_valid_elements_should_succeed()
    {
      // arrange
      var validator = getValidator<TestDummyRequired[]>();
      var array = new[] { new TestDummyRequired { Name = "set" } };

      // act
      var result = await validator.ValidateAsync(array);

      // assert
      result.IsValid.ShouldBeTrue();
    }

    [TestMethod]
    public async Task Validate_array_with_invalid_elements_should_fail()
    {
      // arrange
      var validator = getValidator<TestDummyRequired[]>();
      var array = new[] { new TestDummyRequired { Name = null } };

      // act
      var result = await validator.ValidateAsync(array);

      // assert
      result.IsValid.ShouldBeFalse();
      result.Message.ShouldContain("Following validations for 'CodeBasics.Command.Test.Implementation.DataAnnotationsValidatorFixture+TestDummyRequired[]' failed:");
      result.Message.ShouldContain("Validation of 'CodeBasics.Command.Test.Implementation.DataAnnotationsValidatorFixture+TestDummyRequired' failed");
    }

    private class TestDummyRequired
    {
      [Required]
      public string Name { get; set; }
    }

    private static DataAnnotationsValidator<T> getValidator<T>()
    {
      return new DataAnnotationsValidator<T>(new NullLogger<DataAnnotationsValidator<T>>());
    }

    private static DataAnnotationsValidator<T[]> getValidatorForArray<T>()
    {
      return new DataAnnotationsValidator<T[]>(new NullLogger<DataAnnotationsValidator<T[]>>());
    }
  }
}
