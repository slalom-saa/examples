namespace Authentication.IdentityServer.Models
{
    public class ChangeUserNameInputModel
    {
        public string CurrentEmail { get; set; }

        public string NewEmail { get; set; }

        public string Token { get; set; }
    }
}