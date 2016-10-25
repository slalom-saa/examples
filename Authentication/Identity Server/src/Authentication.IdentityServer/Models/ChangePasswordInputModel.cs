namespace Authentication.IdentityServer.Models
{
    public class ChangePasswordInputModel
    {
        public string UserName { get; set; }

        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }
    }
}