using System.ComponentModel.DataAnnotations;

namespace Celo.RandomUserApi.Controllers.Model
{
    public class ContactInfoDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
    }
}