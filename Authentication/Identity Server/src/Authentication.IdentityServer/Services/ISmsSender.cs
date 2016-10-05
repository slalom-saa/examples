using System;
using System.Threading.Tasks;

namespace Authentication.IdentityServer.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
