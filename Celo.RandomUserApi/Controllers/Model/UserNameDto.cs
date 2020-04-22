using System.ComponentModel.DataAnnotations;

namespace Celo.RandomUserApi.Controllers.Model
{
    public class UserNameDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }
}