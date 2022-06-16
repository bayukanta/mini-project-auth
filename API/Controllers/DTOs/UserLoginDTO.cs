using System.ComponentModel.DataAnnotations;

namespace API.Auth.DTO
{
    public class UserLoginDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
