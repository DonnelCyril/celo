using System;

namespace Celo.RandomUserApi.Infrastructure.ExtensionMethods
{
    public static class ObjectExtensions
    {
        public static void ThrowIfNull(this object argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}