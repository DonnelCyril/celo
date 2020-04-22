using System;
using System.Collections.Generic;
using System.Linq;

namespace Celo.RandomUserApi.Infrastructure.ExtensionMethods
{
    public static class EnumerableExtensions
    {
        public static void ThrowIfNullOrEmpty<T>(this IEnumerable<T> argument, string argumentName)
        {
            if (argument == null || !argument.Any())
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}