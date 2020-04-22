using Celo.RandomUserApi.UserManagement.Model.DeleteUser;
using Celo.RandomUserApi.UserManagement.Model.GetUser;
using Celo.RandomUserApi.UserManagement.Model.UpdateUser;

namespace Celo.RandomUserApi.UserManagement.Model
{
    public class UserNotFound : IGetUserResult, IUpdateUserResult, IUserDeleteResult
    {
        private UserNotFound()
        {

        }
        public static UserNotFound Instance = new UserNotFound();
    }
}