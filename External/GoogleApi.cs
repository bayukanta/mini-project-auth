using AutoMapper;
using External.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using DAL.Model;
using DAL.Repositories;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace External
{
    public class GoogleApi
    {

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public GoogleApi(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClient = httpClientFactory.CreateClient();
            _config = config;
        }

        public async Task<GoogleUserInfoDto> AuthWithGoogleAsync(string tokenType, string accessToken)
        {
            var url = _config.GetValue<string>("GoogleAuthAddress");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
            var httpResult = await _httpClient.GetAsync(url);

            var str = await httpResult.Content.ReadAsStringAsync();

            var resultDto = JsonConvert.DeserializeObject<GoogleUserInfoDto>(str);
            return resultDto;
        }
    }
}
