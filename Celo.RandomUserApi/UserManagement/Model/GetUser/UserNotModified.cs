namespace Celo.RandomUserApi.UserManagement.Model.GetUser
{
    public class UserNotModified : IGetUserResult
    {
        private UserNotModified()
        {
            
        }

        public static UserNotModified Instance = new UserNotModified();
    }
}