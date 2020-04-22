using Celo.RandomUserApi.Infrastructure.ExtensionMethods;

namespace Celo.RandomUserApi.UserManagement.Model.User
{
    public class ContactInfo
    {
        public ContactInfo(string email, string phoneNumber)
        {
            email.ThrowIfNullOrEmpty(nameof(email));
            phoneNumber.ThrowIfNullOrEmpty(nameof(phoneNumber));

            Email = email;
            PhoneNumber = phoneNumber;
        }

        public string Email { get; }
        public string PhoneNumber { get; }
    }
}