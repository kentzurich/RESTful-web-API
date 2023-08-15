using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Newtonsoft.Json;
using System.Text;
using static MagicVilla_Utility.StaticDetails;

namespace MagicVilla_Web.Services
{
    public class APIMessageRequestBuilder : IAPIMessageRequestBuilder
    {
        public HttpRequestMessage Build(APIRequest apiRequest)
        {
            HttpRequestMessage message = new();

            if (apiRequest.ContentType == ContentType.MultipartFormData)
                message.Headers.Add("Accept", "*/*");
            else
                message.Headers.Add("Accept", "application/json");

            message.RequestUri = new Uri(apiRequest.URL);

            if (apiRequest.ContentType == ContentType.MultipartFormData)
            {
                var multipartFormDataContent = new MultipartFormDataContent();

                foreach (var prop in apiRequest.Data.GetType().GetProperties())
                {
                    var value = prop.GetValue(apiRequest.Data);
                    if (value is FormFile)
                    {
                        var file = (FormFile)value;
                        if (file != null)
                            multipartFormDataContent.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
                    }
                    else
                    {
                        multipartFormDataContent.Add(new StringContent(value == null ? string.Empty : value.ToString()), prop.Name);
                    }
                }

                message.Content = multipartFormDataContent;
            }
            else
            {
                if (apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),
                        Encoding.UTF8,
                        "application/json");
                }
            }

            switch (apiRequest.APIType)
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

            return message;
        }
    }
}
