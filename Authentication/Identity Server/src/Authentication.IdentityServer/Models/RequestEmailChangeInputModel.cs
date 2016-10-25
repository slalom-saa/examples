namespace Authentication.IdentityServer.Models
{
    public class RequestEmailChangeInputModel
    {
        public string CurrentEmail { get; set; }

        public string NewEmail { get; set; }
    }
}