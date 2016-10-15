using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Authentication.ConsoleClient
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PostAsJsonAsync(this HttpClient client, string uri, object content)
        {
            return client.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));
        }

        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent message)
        {
            var content = await message.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(content);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Client";

            new Program().Start();

            Console.ReadKey();
        }

        public async void Start()
        {
            using (var client = new IdentityClient(Configuration.Value))
            {
                Console.WriteLine("Calling client...");

                var result = await client.GetUserToken("georgeo@slalom.com", "Pass@word1");

                //var email = "georgeo@slalom.com";

                //var token = await client.RequestPasswordResetAsync(email);

                //var result = await client.ResetPasswordAsync(email, token, "Pass@word1");

                Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
            }
        }

        private static void Register()
        {
            var token = GetClientToken().Result;

            using (var client = new HttpClient())
            {
                client.SetBearerToken(token);

                var result = client.PostAsJsonAsync(Authority + "/identity/actions/register", new
                {
                    Email = "george.olson@windowslive.com",
                    Password = "Pass@word1"
                }).Result;

                Console.Clear();
                Console.WriteLine(OutputContent(result).Result);
                Console.WriteLine();
            }
        }

        private static string RequestWithUserCredentials()
        {
            Console.WriteLine("Getting the user identity...");
            Console.WriteLine();

            var token = GetUserToken("georgeo@slalom.com", "$s1IwhAQ1F").Result;

            using (var client = new HttpClient())
            {
                client.SetBearerToken(token);

                var result = client.GetAsync(WebApiEndpoint + "/identity").Result;

                Console.Clear();
                Console.WriteLine(OutputContent(result).Result);
                Console.WriteLine();
            }
            return token;
        }

        private static void RequestWithClientCredentials()
        {
            Console.WriteLine("Getting the client identity...");

            // connect using a client token
            var token = GetClientToken().Result;

            using (var client = new HttpClient())
            {
                client.SetBearerToken(token);

                var result = client.GetAsync(WebApiEndpoint + "/identity").Result;

                Console.Clear();
                Console.WriteLine(OutputContent(result).Result);
                Console.WriteLine();
            }
        }

        public static async Task<string> OutputContent(HttpResponseMessage message)
        {
            if (message.IsSuccessStatusCode)
            {
                var content = await message.Content.ReadAsStringAsync();
                var temp = JsonConvert.DeserializeObject<JObject>(content);
                return JsonConvert.SerializeObject(temp, Formatting.Indented);
            }
            return message.StatusCode.ToString();
        }

        public static async Task<string> GetClientToken()
        {
            var discovery = await GetDiscoveryInformation();

            var endpoint = discovery.TokenEndpoint;

            var tokenClient = new TokenClient(endpoint, ClientId, ClientSecret);

            var result = await tokenClient.RequestClientCredentialsAsync("api");

            return result.AccessToken;
        }

        private static Task<DiscoveryResponse> GetDiscoveryInformation()
        {
            var client = new DiscoveryClient(Authority + "/.well-known/openid-configuration");

            return client.GetAsync();
        }

        public static async Task<string> GetUserToken(string userName, string password)
        {
            var discovery = await GetDiscoveryInformation();

            var endpoint = discovery.TokenEndpoint;

            var tokenClient = new TokenClient(endpoint, ClientId, ClientSecret);

            var result = await tokenClient.RequestResourceOwnerPasswordAsync(userName, password, "api openid profile roles");

            return result.AccessToken;
        }

        #region Configuration...

        private static readonly Lazy<IConfigurationRoot> Configuration = new Lazy<IConfigurationRoot>(() =>
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            return builder.Build();
        });

        private static string ClientSecret => Configuration.Value["ClientSecret"];

        private static string ClientId => Configuration.Value["ClientId"];

        private static string WebApiEndpoint => Configuration.Value["WebApiEndpoint"];

        private static string Authority => Configuration.Value["Authority"];

        #endregion Configuration
    }
}