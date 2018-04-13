namespace Command.Test
{
    using System;

    using Moq;

    internal static class Mock
    {
        public static T CreateInstanceOf<T>(params object[] args) where T : class
        {
            return CreateInstanceOf((Action<Mock<T>>)null, args);
        }

        public static T CreateInstanceOf<T>(Action<Mock<T>> action, params object[] args) where T : class
        {
            var mock = new Mock<T>(args)
                           {
                               CallBase = true
                           };

            action?.Invoke(mock);

            return mock.Object;
        }
    }
}