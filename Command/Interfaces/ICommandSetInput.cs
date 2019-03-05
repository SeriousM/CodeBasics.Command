namespace CodeBasics.Command
{
  internal interface ICommandSetInput<in TIn>
  {
    void SetInputParameter(TIn value);
  }
}
