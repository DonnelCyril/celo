using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Celo.RandomUserApi.Infrastructure.CosmosDB;
using Celo.RandomUserApi.UserManagement.Model.CreateUser;
using Celo.RandomUserApi.UserManagement.Model.User;
using MediatR;

namespace Celo.RandomUserApi.UserManagement
{
    public class CreateUser : IRequest<CreateUserResult>
    {
        public UserName Name { get; }
        public ContactInfo ContactInfo { get; }
        public DateTime DateOfBirth { get; }
        public List<Uri> ProfileImages { get; }

        public CreateUser(UserName name, ContactInfo contactInfo, DateTime dateOfBirth, List<Uri> profileImages)
        {
            Name = name;
            ContactInfo = contactInfo;
            DateOfBirth = dateOfBirth;
            ProfileImages = profileImages;
        }

        public void Deconstruct(out UserName userName, out ContactInfo contactInfo, out DateTime dateOfBirth, out List<Uri> profileImages)
        {
            userName = Name;
            contactInfo = ContactInfo;
            dateOfBirth = DateOfBirth;
            profileImages = ProfileImages;
        }
    }

    public class CreateUserHandler : IRequestHandler<CreateUser, CreateUserResult>
    {
        private readonly IUserStoreFactory _userStoreFactory;

        public CreateUserHandler(IUserStoreFactory userStoreFactory)
        {
            _userStoreFactory = userStoreFactory;
        }
        public async Task<CreateUserResult> Handle(CreateUser request, CancellationToken cancellationToken)
        {
            var (userName, contactInfo,dateOfBirth, profileImages) = request;
            var user = new User(Guid.NewGuid(), userName, contactInfo, dateOfBirth, profileImages);
            var userStore = await _userStoreFactory.GetUserStore();
            var response = await userStore.CreateItemAsync(user, cancellationToken);
            return new CreateUserResult(response.ETag, user);
        }
    }
}
