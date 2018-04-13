namespace Command.Test.Core
{
    using System.ComponentModel.DataAnnotations;

    public class SampleModel
    {
        [Required]
        public string Name { get; set; }
    }
}