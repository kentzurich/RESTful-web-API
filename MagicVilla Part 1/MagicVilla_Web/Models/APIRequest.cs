using static MagicVilla_Utility.StaticDetails;

namespace MagicVilla_Web.Models
{
    public class APIRequest
    {
        public APIType APIType { get; set; } = APIType.GET;
        public string URL { get; set; }
        public object Data { get; set; }
        public string Token { get; set; }
    }
}
