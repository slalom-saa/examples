namespace Authentication.IdentityServer.Models
{
    public class ResetPasswordInputModel
    {
        public string Email { get; set; }

        public string Token { get; set; }

        public string Password { get; set; }
    }
}