using AutoMapper;
using Celo.RandomUserApi.Controllers.Model;
using Celo.RandomUserApi.UserManagement;
using Celo.RandomUserApi.UserManagement.Model.User;

namespace Celo.RandomUserApi.Infrastructure.AutoMapper.MapperProfiles
{
    public class CreateUserProfile : Profile
    {
        public CreateUserProfile()
        {
            CreateMap<ContactInfoDto, ContactInfo>().ReverseMap();

            CreateMap<UserNameDto, UserName>().ReverseMap();

            CreateMap<CreateUserDto, CreateUser>();
        }

    }
}
