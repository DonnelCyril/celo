using AutoMapper;
using Celo.RandomUserApi.Controllers.Model;
using Celo.RandomUserApi.UserManagement.Model.GetUserList;

namespace Celo.RandomUserApi.Infrastructure.AutoMapper.MapperProfiles
{
    public class UserIndexProfile : Profile
    {
        public UserIndexProfile()
        {
            CreateMap<UserIndex, UserIndexDto>();
        }
        
    }
}