using AutoMapper;
using External.DTOs;
using IdentityModel.Client;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using DAL.Model;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace External
{
    public class IdentityServerApi
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IClientStore _clientStore;

        public IdentityServerApi(IHttpClientFactory httpClientFactory, IConfiguration config, IClientStore clientStore)
        {
            _httpClient = httpClientFactory.CreateClient();
            _config = config;
            _clientStore = clientStore;
        }


        public async Task<TokenResponse> AuthWithIdentityServer(string userName, string password, bool bypassPassword)
        {
            DiscoveryDocumentRequest discoReq = new DiscoveryDocumentRequest()
            {
                Address = "https://localhost:5001",
                Policy = new DiscoveryPolicy()
                {
                    RequireHttps = false,
                    ValidateEndpoints = false,
                    ValidateIssuerName = false
                }
            };

            DiscoveryDocumentResponse discoveryDocument = await _httpClient.GetDiscoveryDocumentAsync(discoReq);


            Client client = await _clientStore.FindEnabledClientByIdAsync(_config.GetValue<string>("Service:ClientId"));

            PasswordTokenRequest passwordTokenRequest = new PasswordTokenRequest()
            {

                Address = discoveryDocument.TokenEndpoint,
                ClientId = _config.GetValue<string>("Service:ClientId"),
                ClientSecret = _config.GetValue<string>("Service:ClientSecret"),
                GrantType = GrantTypes.ResourceOwnerPassword.First(),
                Scope = client.AllowedScopes.Aggregate((p, n) => p + " " + n),
                UserName = userName,
                Password = password
            };

            if (bypassPassword)
            {
                passwordTokenRequest.Parameters.Add("bypassPassword", "true");
            }


            TokenResponse tokenResponse = await _httpClient.RequestPasswordTokenAsync(passwordTokenRequest);
            return tokenResponse;
        }
    }
}
