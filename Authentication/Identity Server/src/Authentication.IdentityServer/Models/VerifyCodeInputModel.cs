using System.ComponentModel.DataAnnotations;

namespace Authentication.IdentityServer.Models
{
    public class VerifyCodeInputModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        public string Code { get; set; }

        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }
}