using Celo.RandomUserApi.Infrastructure.ExtensionMethods;

namespace Celo.RandomUserApi.UserManagement.Model.GetUser
{
    public class UserFound: IGetUserResult
    {
        public UserFound(string etag, User.User user)
        {
            etag.ThrowIfNullOrEmpty(nameof(etag));
            user.ThrowIfNull(nameof(user));

            Etag = etag;
            User = user;
        }

        public string Etag { get; }
        public User.User User { get; }
    }
}
