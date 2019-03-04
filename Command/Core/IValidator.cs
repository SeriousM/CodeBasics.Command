﻿namespace Command.Core
{
  public interface IValidator<in T>
  {
    bool Validate(T value);
  }
}