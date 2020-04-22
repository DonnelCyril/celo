using Celo.RandomUserApi.Infrastructure.ExtensionMethods;

namespace Celo.RandomUserApi.UserManagement.Model.UpdateUser
{
    public class NewerVersionOfUserRecordFound : IUpdateUserResult
    {
        public NewerVersionOfUserRecordFound (string etag)
        {
            etag.ThrowIfNullOrEmpty(nameof(etag));
            Etag = etag;
        } 

        public string Etag { get; }
    }
}