using System.Collections.Generic;
using Celo.RandomUserApi.Infrastructure.ExtensionMethods;

namespace Celo.RandomUserApi.UserManagement.Model.UpdateUser
{
    public class FailedToApplyUserModifications : IUpdateUserResult
    {
        public FailedToApplyUserModifications(IEnumerable<string> errors)
        {
            errors.ThrowIfNullOrEmpty(nameof(errors));
            Errors = errors;
        }

        public IEnumerable<string> Errors { get; }
    }
}