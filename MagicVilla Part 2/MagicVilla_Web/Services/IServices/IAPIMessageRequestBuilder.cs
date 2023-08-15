using MagicVilla_Web.Models;

namespace MagicVilla_Web.Services.IServices
{
    public interface IAPIMessageRequestBuilder
    {
        HttpRequestMessage Build(APIRequest apiRequest);
    }
}
