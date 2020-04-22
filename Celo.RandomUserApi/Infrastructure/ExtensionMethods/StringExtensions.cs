using System;

namespace Celo.RandomUserApi.Infrastructure.ExtensionMethods
{
    public static class StringExtensions
    {
        public static void ThrowIfNullOrEmpty(this string argument, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}