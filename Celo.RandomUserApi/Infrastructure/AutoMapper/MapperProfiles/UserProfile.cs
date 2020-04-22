using AutoMapper;
using Celo.RandomUserApi.Controllers.Model;
using Celo.RandomUserApi.UserManagement.Model.User;

namespace Celo.RandomUserApi.Infrastructure.AutoMapper.MapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}