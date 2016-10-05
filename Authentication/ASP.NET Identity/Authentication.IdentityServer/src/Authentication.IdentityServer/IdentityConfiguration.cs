using System;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4.Models;
using IdentityServer4.Services.InMemory;

namespace Authentication.IdentityServer
{
    public class IdentityConfiguration
    {
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientName = "Example Client",

                    ClientId = "example-client",

                    // no interactive user, use the clientid/secret and/or username/password for authentication
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,

                    // secret for authentication
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("9X$oonapf6".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = new List<string>
                    {
                        "api",
                        StandardScopes.OpenId.Name,
                        StandardScopes.Profile.Name,
                        StandardScopes.Roles.Name
                    }
                }
            };
        }

        public static IEnumerable<Scope> GetScopes()
        {
            return new List<Scope>
            {
                new Scope
                {
                    Name = "api",
                    Description = "Identity API",
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("name", alwaysInclude: true),
                        new ScopeClaim("given_name", alwaysInclude: true),
                        new ScopeClaim("family_name", alwaysInclude: true),
                        new ScopeClaim("email", alwaysInclude: true),
                        new ScopeClaim("role", alwaysInclude: true),
                        new ScopeClaim("website", alwaysInclude: true),
                    }
                },
                StandardScopes.OpenId,
                StandardScopes.Profile,
                StandardScopes.Roles
            };
        }

        public static List<InMemoryUser> GetUsers()
        {
            return new List<InMemoryUser>
            {
                new InMemoryUser
                {
                    Subject = "1",
                    Username = "georgeo@slalom.com",
                    Password = "$s1IwhAQ1F",
                    Claims = new List<Claim>
                    {
                        new Claim("website", "https://github.com/slalom-saa/examples"),
                        new Claim("email", "georgeo@slalom.com")
                    }
                },
                new InMemoryUser
                {
                    Subject = "2",
                    Username = "bob",
                    Password = "password"
                }
            };
        }
    }
}