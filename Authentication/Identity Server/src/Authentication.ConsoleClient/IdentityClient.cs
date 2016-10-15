using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;

namespace Authentication.ConsoleClient
{
    public class IdentityClient : IDisposable
    {
        private readonly Lazy<HttpClient> _client;

        private string _clientSecret;

        private string _clientId;

        private string _authority;

        public async Task<string> RequestPasswordResetAsync(string email)
        {
            var result = await _client.Value.PostAsJsonAsync(_authority + "/identity/actions/request-reset", new
            {
                email
            });
            return await result.Content.ReadAsStringAsync();
        }

        public async Task<string> RequestEmailChangeAsync(string currentEmail, string newEmail)
        {
            var result = await _client.Value.PostAsJsonAsync(_authority + "/identity/actions/request-email-change", new
            {
                currentEmail,
                newEmail
            });
            return await result.Content.ReadAsStringAsync();
        }

        public Task<TokenResponse> GetUserTokenAsync(string userName, string password)
        {
            var client = new TokenClient(_discovery.Value.TokenEndpoint, _clientId, _clientSecret);

            return client.RequestResourceOwnerPasswordAsync(userName, password, "api");
        }

        public async Task<IdentityResponse> ResetPasswordAsync(string email, string token, string password)
        {
            var result = await _client.Value.PostAsJsonAsync(_authority + "/identity/actions/reset-password", new
            {
                email,
                token,
                password
            });
            return await result.Content.ReadAsJsonAsync<IdentityResponse>();
        }


        public async Task<IdentityResponse> ConfirmEmailAsync(string email)
        {
            var result = await _client.Value.PostAsJsonAsync(_authority + "/identity/actions/confirm-email", new
            {
                email
            });
            return await result.Content.ReadAsJsonAsync<IdentityResponse>();
        }

        public IdentityClient(IConfiguration configuration)
        {
            _authority = configuration["Authority"];
            _clientSecret = configuration["ClientSecret"];
            _clientId = configuration["ClientId"];

            _discovery = new Lazy<DiscoveryResponse>(this.GetDiscoveryResponse);
            _token = new Lazy<TokenResponse>(this.GetTokenResponse);
            _client = new Lazy<HttpClient>(this.GetHttpClient);
        }

        private DiscoveryResponse GetDiscoveryResponse()
        {
            var client = new DiscoveryClient(_authority + "/.well-known/openid-configuration");
            return client.GetAsync().Result;
        }

        private HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            client.SetBearerToken(_token.Value.AccessToken);
            return client;
        }

        private TokenResponse GetTokenResponse()
        {
            var client = new TokenClient(_discovery.Value.TokenEndpoint, _clientId, _clientSecret);

            return client.RequestClientCredentialsAsync("api").Result;
        }

        private readonly Lazy<DiscoveryResponse> _discovery;
        private Lazy<TokenResponse> _token;

        public async Task<IdentityResponse> RegisterAsync(string email, string password)
        {
            var result = await _client.Value.PostAsJsonAsync(_authority + "/identity/actions/register", new
            {
                email,
                password
            });

            return await result.Content.ReadAsJsonAsync<IdentityResponse>();
        }

        public void Dispose()
        {
            if (_client.IsValueCreated)
            {
                _client.Value.Dispose();
            }
        }

        public async Task<IdentityResponse> ChangeEmailAsync(string currentEmail, string token, string newEmail)
        {
            var result = await _client.Value.PostAsJsonAsync(_authority + "/identity/actions/change-email", new
            {
                currentEmail,
                newEmail,
                token
            });

            return await result.Content.ReadAsJsonAsync<IdentityResponse>();
        }
    }
}