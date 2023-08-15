using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services
{
    public class VillaNumberService : IVillaNumberService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IBaseService _baseService;
        private string villaURL;

        public VillaNumberService(IHttpClientFactory clientFactory, IConfiguration config, IBaseService baseService)
        {
            _clientFactory = clientFactory;
            _baseService = baseService;
            villaURL = config.GetValue<string>("ServiceUrls:VillaAPI");
        }

        public async Task<T> CreateAsync<T>(VillaNumberCreateDTO dto)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                APIType = StaticDetails.APIType.POST,
                Data = dto,
                URL = $"{villaURL}/api/{StaticDetails.CurrentAPIVersion}/VillaNumberAPI"
            });
        }

        public async Task<T> DeleteAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                APIType = StaticDetails.APIType.DELETE,
                URL = $"{villaURL}/api/{StaticDetails.CurrentAPIVersion}/VillaNumberAPI/{id}"
            });
        }

        public async Task<T> GetAllAsync<T>()
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                APIType = StaticDetails.APIType.GET,
                URL = $"{villaURL}/api/{StaticDetails.CurrentAPIVersion}/VillaNumberAPI"
            });
        }

        public async Task<T> GetAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                APIType = StaticDetails.APIType.GET,
                URL = $"{villaURL}/api/{StaticDetails.CurrentAPIVersion}/VillaNumberAPI/{id}"
            });
        }

        public Task<T> UpdateAsync<T>(VillaNumberUpdateDTO dto)
        {
            return _baseService.SendAsync<T>(new APIRequest()
            {
                APIType = StaticDetails.APIType.PUT,
                Data = dto,
                URL = $"{villaURL}/api/{StaticDetails.CurrentAPIVersion}/VillaNumberAPI/{dto.VillaNo}"
            });
        }
    }
}
