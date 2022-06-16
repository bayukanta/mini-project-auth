using System;

namespace API.Auth.DTO
{
    public class UserInfoTokenDTO
    {
        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public UserTokenDTO TokenResponse { get; set; }
    }
}
