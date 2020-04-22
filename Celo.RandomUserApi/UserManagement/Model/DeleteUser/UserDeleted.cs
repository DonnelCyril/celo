namespace Celo.RandomUserApi.UserManagement.Model.DeleteUser
{
    public class UserDeleted: IUserDeleteResult
    {
        private UserDeleted()
        {
            
        }

        public static UserDeleted Instance = new UserDeleted();
    }
}