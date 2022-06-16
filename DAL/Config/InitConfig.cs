using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ApiResource = IdentityServer4.Models.ApiResource;
using Client = IdentityServer4.Models.Client;
using IdentityResource = IdentityServer4.Models.IdentityResource;
using Secret = IdentityServer4.Models.Secret;

namespace DAL.Config
{
    public static class InitConfig
    {

        public static void InitializeDatabase(IApplicationBuilder app)
        {
            using (IServiceScope serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                ConfigurationDbContext context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();

                if (!context.Clients.Any());
                {
                    foreach (Client client in GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (IdentityResource resource in GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (ApiScope resource in GetApiScopes())
                    {
                        context.ApiScopes.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (ApiResource resource in GetApis())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }

        public static List<TestUser> GetUsers()
        {
            List<TestUser> Users = new List<TestUser>{
            new TestUser{SubjectId = "818727", Username = "alice", Password = "alice",
                Claims =
                {
                    new Claim(JwtClaimTypes.Name, "Alice Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Alice"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                    new Claim("username", "AliceSmith@email.com"),
                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                    new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                }
            },
            new TestUser{SubjectId = "88421113", Username = "bob", Password = "bob",
                Claims =
                {
                    new Claim(JwtClaimTypes.Name, "Bob Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Bob"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                    new Claim("username", "BobSmith@email.com"),
                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                    new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                    new Claim("location", "somewhere")
                }
            },
            new TestUser{SubjectId = "1234567", Username = "bayu", Password = "bayu",
                Claims =
                {
                    new Claim(JwtClaimTypes.Name, "bayu"),
                    new Claim(JwtClaimTypes.GivenName, "bayu"),
                    new Claim(JwtClaimTypes.FamilyName, "bayu"),
                    new Claim(JwtClaimTypes.Email, "bayu@gmail.com"),
                    new Claim("username", "bayu@gmail.com"),
                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new Claim(JwtClaimTypes.WebSite, "http://bayu.com"),
                    new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                    new Claim("location", "somewhere")
                }
            }
        };
            return Users;
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource()
                 {
                     Name="role",
                     UserClaims=new List<string>{"role"}
                 },
                new IdentityResource()
                 {
                     Name="userId",
                     UserClaims=new List<string>{"userId"}
                 },
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource("AuthService")
                {
                    Scopes = { "auth" }
                },
                new ApiResource("Service")
                {
                    Scopes = { "service" }
                },
                new ApiResource("SampleMobile")
                {
                    Scopes = { "mobile" }
                },
                new ApiResource("SampleWeb")
                {
                    Scopes = { "web" }
                }
,
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new ApiScope[]
            {
                new ApiScope("auth"),
                new ApiScope("web"),
                new ApiScope("mobile"),
                new ApiScope("service"),
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                new Client
                {
                    ClientId ="AuthService",
                    ClientName = "Client Credentials Name",

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = { new Secret("ClientSecret".Sha256()) },
                    AllowedScopes = { "openid", "profile","auth" }
                },
                new Client
                {
                    ClientId ="Service",
                    ClientName = "Client Credentials Name",

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = { new Secret("ClientSecret".Sha256()) },
                    AllowedScopes = { "openid", "profile","service","auth" }
                },
            };
        }
    }
}
