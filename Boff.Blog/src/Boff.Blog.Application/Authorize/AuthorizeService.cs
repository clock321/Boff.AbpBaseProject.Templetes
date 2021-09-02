using Boff.Blog.Configurations;
using Boff.Blog.Domain.Configurations;
using Boff.Blog.ToolKits.Extensions;
using Boff.Blog.ToolKits.GitHub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace Boff.Blog.Authorize
{
    public interface IAuthorizeService
    {
        /// <summary>
        /// 获取登录地址(GitHub)
        /// </summary>
        /// <returns></returns>
        Task<string> GetLoginAddressAsync();

        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<string> GetAccessTokenAsync(string code);

        /// <summary>
        /// 登录成功，生成Token
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        Task<string> GenerateTokenAsync(string access_token);
    }

    /// <summary>
    /// JWT模式认证授权
    /// </summary>
    [AllowAnonymous]
    public class AuthorizeService : BlogAppServiceBase, IAuthorizeService
    {
        private readonly IHttpClientFactory _httpClient;

        public AuthorizeService(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }


        /// <summary>
        /// 登录成功，生成Token
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public async Task<string> GenerateTokenAsync(string access_token)
        {      
            if (string.IsNullOrEmpty(access_token))
            {
                throw new UserFriendlyException("access_token为空");
            }

            var url = $"{GitHubConfig.API_User}";
            using var client = _httpClient.CreateClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.14 Safari/537.36 Edg/83.0.478.13");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {access_token}");
            var httpResponse = await client.GetAsync(url);
            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new UserFriendlyException("access_token不正确");
            }

            var content = await httpResponse.Content.ReadAsStringAsync();

            var user = content.FromJson<UserResponse>();
            if (user.IsNull())
            {
                throw new UserFriendlyException("未获取到用户数据");
            }

            if (user.id != GitHubConfig.UserId)
            {
                throw new UserFriendlyException("当前账号未授权");
            }

            var claims = new[] {
                    new Claim(ClaimTypes.Name, user.login??""),
                    new Claim(ClaimTypes.Email, user.email??""),
                    new Claim(JwtRegisteredClaimNames.Exp, $"{new DateTimeOffset(DateTime.Now.AddMinutes(AppSettings.JWT.Expires)).ToUnixTimeSeconds()}"),
                    new Claim(JwtRegisteredClaimNames.Nbf, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}")
                };

            var key = new SymmetricSecurityKey(AppSettings.JWT.SecurityKey.SerializeUtf8());
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var securityToken = new JwtSecurityToken(
                issuer: AppSettings.JWT.Domain,
                audience: AppSettings.JWT.Domain,
                claims: claims,
                expires: DateTime.Now.AddMinutes(AppSettings.JWT.Expires),
                signingCredentials: creds);

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return await Task.FromResult(token);
        }

        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<string> GetAccessTokenAsync(string code)
        {     
            if (string.IsNullOrEmpty(code))
            {
                throw new UserFriendlyException("code为空");
            }

            var request = new AccessTokenRequest();

            var content = new StringContent($"code={code}&client_id={request.Client_ID}&redirect_uri={request.Redirect_Uri}&client_secret={request.Client_Secret}");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            using var client = _httpClient.CreateClient();
            var httpResponse = await client.PostAsync(GitHubConfig.API_AccessToken, content);

            var response = await httpResponse.Content.ReadAsStringAsync();

            if (response.StartsWith("access_token"))
                return response.Split("=")[1].Split("&").First();
            else
                throw new UserFriendlyException("code不正确");

        }


        /// <summary>
        /// 获取登录地址(GitHub)
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetLoginAddressAsync()
        {
            var request = new AuthorizeRequest();
            var address = string.Concat(new string[]
            {
                    GitHubConfig.API_Authorize,
                    "?client_id=", request.Client_ID,
                    "&scope=", request.Scope,
                    "&state=", request.State,
                    "&redirect_uri=", request.Redirect_Uri
            });
            return await Task.FromResult(address);
        }
    }
}
