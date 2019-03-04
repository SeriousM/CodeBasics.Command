using Command.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Command.Test
{
  [TestClass]
  public class ResultShould
  {
    [TestMethod]
    public void ReturnFailStatusWhenResultIsConstruct()
    {
      var result = Result<object>.ExecutionFail("failed");
      result.Status.ShouldBe(Status.Fail);
    }

    [TestMethod]
    public void ReturnInstanceOfMessagesWhenResultIsConstruct()
    {
      var result = Result<object>.ExecutionFail("failed");
      result.Messages.ShouldNotBeNull();
    }
  }
}
