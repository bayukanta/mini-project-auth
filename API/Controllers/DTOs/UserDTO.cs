using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Auth.DTO
{
    public class UserDTO
    {

        public Guid? UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string Password { get; set; }

        public string LastName { get; set; }

        public List<Guid> RoleIds { get; set; }

    }
}
