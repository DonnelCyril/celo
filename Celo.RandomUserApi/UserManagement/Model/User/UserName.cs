using Celo.RandomUserApi.Infrastructure.ExtensionMethods;

namespace Celo.RandomUserApi.UserManagement.Model.User
{
    public class UserName
    {
        public UserName(string title, string firstName, string lastName)
        {
            title.ThrowIfNullOrEmpty(nameof(title));
            firstName.ThrowIfNullOrEmpty(nameof(firstName));
            lastName.ThrowIfNullOrEmpty(nameof(lastName));

            Title = title;
            FirstName = firstName;
            LastName = lastName;
        }

        public string Title { get; }
        public string FirstName { get; }
        public string LastName { get; }
    }
}