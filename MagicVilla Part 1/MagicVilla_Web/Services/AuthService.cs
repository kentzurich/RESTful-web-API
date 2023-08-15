using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string villaURL;

        public AuthService(IHttpClientFactory clientFactory, IConfiguration config) : base(clientFactory)
        {
            _clientFactory = clientFactory;
            villaURL = config.GetValue<string>("ServiceUrls:VillaAPI");
        }

        public Task<T> LoginAsync<T>(LoginRequestDTO loginRequestDTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                APIType = StaticDetails.APIType.POST,
                Data = loginRequestDTO,
                URL = $"{villaURL}/api/v1/UserAuth/login"
            });
        }

        public Task<T> RegisterAsync<T>(RegistrationRequestDTO userDTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                APIType = StaticDetails.APIType.POST,
                Data = userDTO,
                URL = $"{villaURL}/api/v1/UserAuth/register"
            });
        }
    }
}
