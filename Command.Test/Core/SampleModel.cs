using System.ComponentModel.DataAnnotations;

namespace Command.Test.Core
{
  public class SampleModel
  {
    [Required]
    public string Name { get; set; }
  }
}
