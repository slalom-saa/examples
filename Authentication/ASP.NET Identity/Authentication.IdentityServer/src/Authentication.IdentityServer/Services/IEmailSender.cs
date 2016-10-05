using System;
using System.Threading.Tasks;

namespace Authentication.IdentityServer.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
