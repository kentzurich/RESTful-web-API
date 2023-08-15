using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using static MagicVilla_Utility.StaticDetails;

namespace MagicVilla_Web.Services
{
    public class VillaService : IVillaService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IBaseService _baseService;
        private string villaURL;

        public VillaService(IHttpClientFactory clientFactory, IConfiguration config, IBaseService baseService)
        {
            _clientFactory = clientFactory;
            _baseService = baseService;
            villaURL = config.GetValue<string>("ServiceUrls:VillaAPI");
        }

        public Task<T> CreateAsync<T>(VillaCreateDTO dto)
        {
            return _baseService.SendAsync<T>(new APIRequest()
            {
                APIType = StaticDetails.APIType.POST,
                Data = dto,
                URL = $"{villaURL}/api/{StaticDetails.CurrentAPIVersion}/VillaAPI",
                ContentType = ContentType.MultipartFormData
            });
        }

        public async Task<T> DeleteAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                APIType = StaticDetails.APIType.DELETE,
                URL = $"{villaURL}/api/{StaticDetails.CurrentAPIVersion}/VillaAPI/{id}"
            });
        }

        public async Task<T> GetAllAsync<T>()
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                APIType = StaticDetails.APIType.GET,
                URL = $"{villaURL}/api/{StaticDetails.CurrentAPIVersion}/VillaAPI"
            });
        }

        public async Task<T> GetAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                APIType = StaticDetails.APIType.GET,
                URL = $"{villaURL}/api/{StaticDetails.CurrentAPIVersion}/VillaAPI/{id}"
            });
        }

        public async Task<T> UpdateAsync<T>(VillaUpdateDTO dto)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                APIType = StaticDetails.APIType.PUT,
                Data = dto,
                URL = $"{villaURL}/api/{StaticDetails.CurrentAPIVersion}/VillaAPI/{dto.Id}",
                ContentType = ContentType.MultipartFormData
            });
        }
    }
}
