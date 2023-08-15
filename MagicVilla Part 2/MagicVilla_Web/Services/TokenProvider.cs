using MagicVilla_Utility;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public void ClearToken()
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(StaticDetails.AccessToken);
            _contextAccessor.HttpContext?.Response.Cookies.Delete(StaticDetails.RefreshToken);
        }

        public TokenDTO GetToken()
        {
            try
            {
                bool hasAccessToken = _contextAccessor.HttpContext.Request.Cookies.TryGetValue(StaticDetails.AccessToken, out var accessToken);
                bool hasRefreshToken = _contextAccessor.HttpContext.Request.Cookies.TryGetValue(StaticDetails.RefreshToken, out var refreshToken);
                TokenDTO tokenDTO = new()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
                return hasAccessToken ? tokenDTO : null;
            }
            catch 
            { 
                return null;
            }
        }

        public void SetToken(TokenDTO tokenDTO)
        {
            var cookieOptions = new CookieOptions { Expires = DateTime.UtcNow.AddDays(60) };
            _contextAccessor.HttpContext?.Response.Cookies.Append(StaticDetails.AccessToken, tokenDTO.AccessToken, cookieOptions);
            _contextAccessor.HttpContext?.Response.Cookies.Append(StaticDetails.RefreshToken, tokenDTO.RefreshToken, cookieOptions);
        }
    }
}
