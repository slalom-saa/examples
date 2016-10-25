using System;
using System.Web;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Authentication.IdentityServer.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace Authentication.IdentityServer.Api
{
    [RoutePrefix("api/identity")]
    public class IdentityController : ApiController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public IdentityController()
        {
        }

        public IdentityController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            this.UserManager = userManager;
            this.SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>(); }
            private set { _signInManager = value; }
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.Current.GetOwinContext().Authentication; }
        }

        [HttpPost, Route("sign-in")]
        public async Task<dynamic> SignIn(LoginViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var result = await this.SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);

            return result.ToString();
        }

        [HttpPost, Route("sign-out"), Authorize]
        public void SignOut()
        {
            this.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }
    }
}