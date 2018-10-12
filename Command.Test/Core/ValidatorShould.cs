namespace Command.Test.Core
{
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

    using Mock = Test.Mock;

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

            var result = validator.Validate(null);

            logger.Verify();
            result.Messages.ShouldContain("The value can not be null.");
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

            var result = validator.Validate(new SampleModel { Name = string.Empty });

            logger.Verify();
            result.Messages.ShouldContain(string.Format(expectedMessage, "Name"));
        }

        [Test]
        public void ReturnFailWhenDataAnnotationOnValueIsNotValid()
        {
            var logger = new Mock<ILogger<Validator<SampleModel>>>();
            var validator = new Validator<SampleModel>(logger.Object);

            var result = validator.Validate(new SampleModel { Name = string.Empty });

            result.Status.ShouldBe(Status.Fail);
        }

        [Test]
        public void ReturnFailWhenDataAnnotationOnValueIsValidButOnValidateReturnFalse()
        {
            var sampleModel = new SampleModel { Name = "correct Value" };
            var logger = new Mock<ILogger<Validator<SampleModel>>>();
            var validator = Mock.CreateInstanceOf<Validator<SampleModel>>(
                m => m.Setup(v => v.OnValidate(sampleModel)).Returns(new Result()),
                logger.Object);

            var result = validator.Validate(sampleModel);

            result.Status.ShouldBe(Status.Fail);
        }

        [Test]
        public void ReturnFailWhenValueIsRefTypeAndNull()
        {
            var logger = new Mock<ILogger<Validator<List<int>>>>();
            var validator = new Validator<List<int>>(logger.Object);

            var result = validator.Validate(null);

            result.Status.ShouldBe(Status.Fail);
        }

        [Test]
        public void ReturnSuccessWhenDataAnnotationOnValueIsValid()
        {
            var logger = new Mock<ILogger<Validator<SampleModel>>>();
            var validator = new Validator<SampleModel>(logger.Object);

            var result = validator.Validate(new SampleModel { Name = "correct value" });

            result.Status.ShouldBe(Status.Success);
        }

        [Test]
        public void ReturnSuccessWhenValueIsByRefAndNotNull()
        {
            var validator = new Validator<List<int>>(It.IsAny<ILogger<Validator<List<int>>>>());

            var result = validator.Validate(new List<int>());

            result.Status.ShouldBe(Status.Success);
        }

        [Test]
        public void ReturnSuccessWhenValueIsPrimitiveType()
        {
            var validator = new Validator<string>(It.IsAny<ILogger<Validator<string>>>());

            var result = validator.Validate(It.IsAny<string>());

            result.Status.ShouldBe(Status.Success);
        }

        [Test]
        public void ReturnSuccessWhenValueNullableType()
        {
            var validator = new Validator<int?>(It.IsAny<ILogger<Validator<int?>>>());

            var result = validator.Validate(null);

            result.Status.ShouldBe(Status.Success);
        }
    }
}