using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IBaseService _baseService;
        private string villaURL;

        public AuthService(IHttpClientFactory clientFactory, IConfiguration config, IBaseService baseService)
        {
            _clientFactory = clientFactory;
            _baseService = baseService;
            villaURL = config.GetValue<string>("ServiceUrls:VillaAPI");
        }

        public async Task<T> LoginAsync<T>(LoginRequestDTO loginRequestDTO)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                APIType = StaticDetails.APIType.POST,
                Data = loginRequestDTO,
                URL = $"{villaURL}/api/{StaticDetails.CurrentAPIVersion}/UserAuth/login"
            }, withBearer:false);
        }

        public async Task<T> LogoutAsync<T>(TokenDTO tokenDTO)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                APIType = StaticDetails.APIType.POST,
                Data = tokenDTO,
                URL = $"{villaURL}/api/{StaticDetails.CurrentAPIVersion}/UserAuth/Revoke"
            }, withBearer: false);
        }

        public async Task<T> RegisterAsync<T>(RegistrationRequestDTO userDTO)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                APIType = StaticDetails.APIType.POST,
                Data = userDTO,
                URL = $"{villaURL}/api/{StaticDetails.CurrentAPIVersion}/UserAuth/register"
            }, withBearer: false);
        }
    }
}
