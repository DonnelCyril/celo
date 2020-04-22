using System;
using System.Collections.Generic;

namespace Celo.RandomUserApi.Controllers.Model
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public UserNameDto Name { get; set; }
        public ContactInfoDto ContactInfo { get; set; }
        public DateTime DateOfBirth { get; set; }
        public IEnumerable<Uri> ProfileImages { get; set; }

    }
}