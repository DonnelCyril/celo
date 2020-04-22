using System;
using System.Linq;
using Celo.RandomUserApi.UserManagement.Model;
using Celo.RandomUserApi.UserManagement.Model.UpdateUser;

namespace Celo.RandomUserApi.UnitTests.UserUpdate.TestFixture
{
    public static class CompareUpdateUserResults
    {
        public static bool IsEquivalentTo(this IUpdateUserResult actual, IUpdateUserResult expected) =>
            actual switch
            {
                FailedToApplyUserModifications a when expected is FailedToApplyUserModifications e => a.Errors.All(e.Errors.Contains),
                NewerVersionOfUserRecordFound a when expected is NewerVersionOfUserRecordFound e => a.Etag.Equals(e.Etag, StringComparison.InvariantCultureIgnoreCase),
                UserSuccessfullyUpdated a when expected is UserSuccessfullyUpdated e => a.Etag.Equals(e.Etag, StringComparison.InvariantCultureIgnoreCase),
                UserNotFound a when expected is UserNotFound e => a.Equals(e),
                _ => false
            };
    }
}