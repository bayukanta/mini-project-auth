namespace API.Auth.DTO
{
    public class UserTokenDTO
    {
        public string AccessToken { get; set; }

        public string IdentityToken { get; set; }

        public string TokenType { get; set; }

        public string RefreshToken { get; set; }

        public string ErrorDescription { get; set; }

        public int ExpiresIn { get; set; }
    }
}
