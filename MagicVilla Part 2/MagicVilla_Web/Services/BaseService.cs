using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_Web.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse responseModel { get; set; }
        public IHttpClientFactory httpClient { get; set; }
        private readonly ITokenProvider _tokenProvider;
        private readonly string villaAPIUrl;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IAPIMessageRequestBuilder _apiMessageRequestBuilder;

        public BaseService(IHttpClientFactory httpClient, 
                           ITokenProvider tokenProvider,
                           IConfiguration config,
                           IHttpContextAccessor httpContextAccessor,
                           IAPIMessageRequestBuilder apiMessageRequestBuilder)
        {
            this.responseModel = new();
            this.httpClient = httpClient;
            _tokenProvider = tokenProvider;
            villaAPIUrl = config.GetValue<string>("ServiceUrls:VillaAPI");
            _httpContextAccessor = httpContextAccessor;
            _apiMessageRequestBuilder = apiMessageRequestBuilder;
        }

        public async Task<T> SendAsync<T>(APIRequest apiRequest, bool withBearer = true)
        {
            try
            {
                var client = httpClient.CreateClient("MagicAPI");

                var messageFactory = () => _apiMessageRequestBuilder.Build(apiRequest);

                HttpResponseMessage httpResponseMessage = null;
                httpResponseMessage = await SendWithResfreshToken(client, messageFactory, withBearer);

                APIResponse finalAPIResponse = new() { IsSuccess = false };

                try
                {
                    switch(httpResponseMessage.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                            finalAPIResponse.ErrorMessages = new List<string>() { "Not Found" };
                            break;
                        case HttpStatusCode.Forbidden:
                            finalAPIResponse.ErrorMessages = new List<string>() { "Forbidden" };
                            break;
                        case HttpStatusCode.Unauthorized:
                            finalAPIResponse.ErrorMessages = new List<string>() { "Unauthorized" };
                            break;
                        case HttpStatusCode.InternalServerError:
                            finalAPIResponse.ErrorMessages = new List<string>() { "Internal Server Error" };
                            break;
                        default:
                            var content = await httpResponseMessage.Content.ReadAsStringAsync();
                            finalAPIResponse.IsSuccess = true;
                            finalAPIResponse = JsonConvert.DeserializeObject<APIResponse>(content);
                            break;
                    }
                }
                catch(Exception ex)
                {
                    finalAPIResponse.ErrorMessages = new List<string> { "Error Encountered", ex.Message.ToString() };
                }

                var responseObj = JsonConvert.SerializeObject(finalAPIResponse);
                var returnObj = JsonConvert.DeserializeObject<T>(responseObj);

                return returnObj;
            }
            catch (AuthException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var dto = new APIResponse()
                {
                    ErrorMessages = new List<string> { ex.InnerException.ToString() },
                    IsSuccess = false
                };

                var response = JsonConvert.SerializeObject(dto);
                var apiResponse = JsonConvert.DeserializeObject<T>(response);

                return apiResponse;
            }
        }

        private async Task<HttpResponseMessage> SendWithResfreshToken(HttpClient httpClient,
            Func<HttpRequestMessage> httpRequestMessageFactory,
            bool withBearer = true)
        {
            if (!withBearer)
            {
                return await httpClient.SendAsync(httpRequestMessageFactory());
            }
            else
            {
                TokenDTO tokenDTO = _tokenProvider.GetToken();
                if(tokenDTO != null && !string.IsNullOrEmpty(tokenDTO.AccessToken))
                    httpClient.DefaultRequestHeaders.Authorization = new("Bearer", tokenDTO.AccessToken);

                try
                {
                    var response = await httpClient.SendAsync(httpRequestMessageFactory());
                    if (response.IsSuccessStatusCode)
                        return response;

                    //if this fails then we can pass refresh token
                    if(!response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        //generate new token from refresh token / sign in with that new token and then retry
                        await InvokeRefreshTokenEndpoint(httpClient, tokenDTO.AccessToken, tokenDTO.RefreshToken);
                        response = await httpClient.SendAsync(httpRequestMessageFactory());
                        return response;
                    }

                    return response;
                }
                catch(AuthException)
                {
                    throw;
                }
                catch(HttpRequestException httpRequestException) 
                { 
                    if(httpRequestException.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        //refresh token and retry the request
                        await InvokeRefreshTokenEndpoint(httpClient, tokenDTO.AccessToken, tokenDTO.RefreshToken);
                        return await httpClient.SendAsync(httpRequestMessageFactory());
                    }
                    throw;
                }
            }
        }

        private async Task InvokeRefreshTokenEndpoint(HttpClient httpClient, string existingAccessToken, string existingRefreshToken)
        {
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");
            message.RequestUri = new Uri($"{villaAPIUrl}/api/{StaticDetails.CurrentAPIVersion}/UserAuth/Refresh");
            message.Method = HttpMethod.Post;
            message.Content = new StringContent(JsonConvert.SerializeObject(new TokenDTO()
            {
                AccessToken = existingAccessToken,
                RefreshToken = existingRefreshToken
            }), Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(message);
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<APIResponse>(content);

            if(apiResponse?.IsSuccess != true)
            {
                await _httpContextAccessor.HttpContext.SignOutAsync();
                _tokenProvider.ClearToken();
                throw new AuthException();
            }
            else
            {
                var tokenDataString = JsonConvert.SerializeObject(apiResponse.Result);
                var tokenDTO = JsonConvert.DeserializeObject<TokenDTO>(tokenDataString);

                if (tokenDTO != null && !string.IsNullOrEmpty(tokenDTO.AccessToken))
                {
                    //new method to sign in with the new token that we receive
                    await SignInWithNewTokens(tokenDTO);
                    httpClient.DefaultRequestHeaders.Authorization = new("Bearer", tokenDTO.AccessToken);
                }
            }  
        }

        private async Task SignInWithNewTokens(TokenDTO tokenDTO)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(tokenDTO.AccessToken);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(x => x.Type == "unique_name").Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(x => x.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            _tokenProvider.SetToken(tokenDTO);
        }
    }
}
