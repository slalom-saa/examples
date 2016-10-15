using System;
using System.Threading.Tasks;
using Authentication.IdentityServer.Data;
using Authentication.IdentityServer.Models;
using Authentication.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.IdentityServer.Controllers
{
    [Authorize]
    public class IdentityController : ControllerBase
    {
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ISmsSender _smsSender;
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, ISmsSender smsSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
        }

        [Route("identity/actions/register"), HttpPost]
        public async Task<IdentityResult> Register([FromBody] RegisterInputModel request)
        {
            if (!this.ModelState.IsValid)
            {
                throw new InvalidOperationException();
            }

           return await _userManager.CreateAsync(new ApplicationUser { Email = request.Email, UserName = request.Email }, request.Password);
        }

        [Route("identity/actions/confirm-email"), HttpPost]
        public async Task<IdentityResult> ConfirmEmail([FromBody]ConfirmEmailInputModel request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            user.EmailConfirmed = true;

            return await _userManager.UpdateAsync(user);
        }

        [Route("identity/actions/request-reset"), HttpPost]
        public async Task<string> RequestReset([FromBody]RequestPasswordResultInputModel request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user.EmailConfirmed)
            {
                return await _userManager.GeneratePasswordResetTokenAsync(user);
            }
            throw new InvalidOperationException("The specified user has not confirmed email.");
        }

        [Route("identity/actions/reset-password"), HttpPost]
        public async Task<IdentityResult> ResetPassword([FromBody] ResetPasswordInputModel request)
        {
            if (!this.ModelState.IsValid)
            {
                throw new InvalidOperationException();
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            return await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
        }

        [Route("identity/actions/change-passsword"), HttpPost]
        public async Task ChangePassword([FromBody] ChangePasswordInputModel request)
        {
            if (!this.ModelState.IsValid)
            {
                throw new InvalidOperationException();
            }

            var user = await _userManager.FindByNameAsync(request.UserName);
            await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        }

        [Route("identity/actions/change-email"), HttpGet]
        public async Task RequestEmailUpdate([FromBody] ChangeUserNameInputModel request)
        {
            if (!this.ModelState.IsValid)
            {
                throw new InvalidOperationException();
            }

            var user = await _userManager.FindByNameAsync(request.CurrentEmail);
            await _userManager.GenerateChangeEmailTokenAsync(user, request.NewEmail);
        }

        [Route("identity/actions/change-email"), HttpPost]
        public async Task UpdateEmail([FromBody] ChangeUserNameInputModel request)
        {
            if (!this.ModelState.IsValid)
            {
                throw new InvalidOperationException();
            }

            var user = await _userManager.FindByNameAsync(request.CurrentEmail);
            await _userManager.ChangeEmailAsync(user, request.NewEmail, request.Token);
        }
    }
}