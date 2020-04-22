using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Celo.RandomUserApi.Controllers.Model
{
    public class CreateUserDto
    {
        [Required]
        public UserNameDto Name { get; set; }
        [Required]
        public ContactInfoDto ContactInfo { get; set; }
        [WithinHundredYearsFromNow]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public List<Uri> ProfileImages { get; set; }

    }

    public class WithinHundredYearsFromNowAttribute : RangeAttribute
    {
        public WithinHundredYearsFromNowAttribute() : base(
            typeof(DateTime),
            DateTime.Now.AddYears(-100).ToShortDateString(),
            DateTime.Now.ToShortDateString())
        {

        }
    }
}