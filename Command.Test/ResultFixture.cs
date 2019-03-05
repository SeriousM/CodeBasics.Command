using Command.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Command.Test
{
  [TestClass]
  public class ResultFixture
  {
    [TestMethod]
    public void ReturnFailStatusWhenResultIsConstruct()
    {
      var result = Result<object>.PostValidationFail("failed");
      result.Status.ShouldBe(Status.PostValidationFalied);
    }

    [TestMethod]
    public void ReturnInstanceOfMessagesWhenResultIsConstruct()
    {
      var result = Result<object>.PreValidationFail("failed");
      result.Messages.ShouldNotBeNull();
    }
  }
}
