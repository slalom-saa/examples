namespace Authentication.IdentityServer.Models
{
    public class ChangeEmailInputModel
    {
        public string CurrentEmail { get; set; }

        public string NewEmail { get; set; }

        public string Token { get; set; }
    }
}