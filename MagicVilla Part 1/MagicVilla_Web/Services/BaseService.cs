using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace MagicVilla_Web.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse responseModel { get; set; }
        public IHttpClientFactory httpClient { get; set; }
        public BaseService(IHttpClientFactory httpClient)
        {
            this.responseModel = new();
            this.httpClient = httpClient;
        }

        public async Task<T> SendAsync<T>(APIRequest apiRequest)
        {
            try
            {
                var client = httpClient.CreateClient("MagicAPI");
                HttpRequestMessage message = new();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.URL);

                if(apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),
                        Encoding.UTF8,
                        "application/json");
                }
                
                switch(apiRequest.APIType)
                {
                    case StaticDetails.APIType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case StaticDetails.APIType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case StaticDetails.APIType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                HttpResponseMessage response = null;

                if (!string.IsNullOrEmpty(apiRequest.Token))
                    client.DefaultRequestHeaders.Authorization = new("Bearer", apiRequest.Token);

                response = await client.SendAsync(message);

                var content = await response.Content.ReadAsStringAsync();

                try
                {
                    APIResponse APIResp = JsonConvert.DeserializeObject<APIResponse>(content);
                    if(response!= null && (response.StatusCode == HttpStatusCode.BadRequest ||
                        response.StatusCode == HttpStatusCode.NotFound))
                    {
                        APIResp.StatusCode = HttpStatusCode.BadRequest;
                        APIResp.IsSuccess = false;

                        var responseObj = JsonConvert.SerializeObject(APIResp);
                        var returnObj = JsonConvert.DeserializeObject<T>(responseObj);

                        return returnObj;
                    }
                }
                catch(Exception ex)
                {
                    var exceptionAPIResponse = JsonConvert.DeserializeObject<T>(content);
                    return exceptionAPIResponse;
                }

                var apiResponse = JsonConvert.DeserializeObject<T>(content);
                return apiResponse;
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
    }
}
