using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/UserAuth")]
    [ApiController]
    [ApiVersionNeutral]
    public class UserController : Controller
    {
        private readonly IUserRepository _user;
        protected APIResponse _response;

        public UserController(IUserRepository user)
        {
            _user = user;
        }

        [HttpGet("Error")]
        public async Task<IActionResult> Error()
        {
            throw new FileNotFoundException();
        }

        [HttpGet("ImageError")]
        public async Task<IActionResult> ImageError()
        {
            throw new BadImageFormatException("Fake image exception.");
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var tokenDTO = await _user.Login(model);
            if (tokenDTO == null || string.IsNullOrEmpty(tokenDTO.AccessToken))
            {
                _response = APIResponse(HttpStatusCode.BadRequest, false, new List<string> { "Username or Password is incorrect." });
                return BadRequest(_response);
            }

            _response = APIResponse(HttpStatusCode.OK, true, Result: tokenDTO);
            return Ok(_response);
        }

        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            bool isUniqueUser = await _user.IsUniqueUser(model.UserName);
            if (!isUniqueUser)
            {
                _response = APIResponse(HttpStatusCode.BadRequest, false, new List<string> { "Username already exists." });
                return BadRequest(_response);
            }

            var user = await _user.Register(model);
            if (user == null || user.UserName == null)
            {
                _response = APIResponse(HttpStatusCode.BadRequest, false, new List<string> { "Error while registering the user." });
                return BadRequest(_response);
            }

            _response = APIResponse(HttpStatusCode.OK);
            return Ok(_response);
        }

        [HttpPost("Refresh")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNewTokenFromRefreshToken([FromBody] TokenDTO tokenDTO)
        {
            if(ModelState.IsValid)
            {
                var tokenDTOResponse = await _user.RefreshAccessToken(tokenDTO);
                if (tokenDTOResponse == null || string.IsNullOrEmpty(tokenDTOResponse.AccessToken))
                {
                    _response = APIResponse(HttpStatusCode.BadRequest, false, new List<string> { "Token invalid." });
                    return BadRequest(_response);
                }

                _response = APIResponse(HttpStatusCode.OK, Result:tokenDTOResponse);
                return Ok(_response);
            }
            else
            {
                _response = APIResponse(HttpStatusCode.BadRequest, false, new List<string> { "Invalid input." });
                return BadRequest(_response);
            }
        }

        [HttpPost("Revoke")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] TokenDTO tokenDTO)
        {
            if (ModelState.IsValid)
            {
                await _user.RevokeRefreshToken(tokenDTO);
                _response = APIResponse(HttpStatusCode.OK, true);
                return Ok(_response);
            }

            _response = APIResponse(HttpStatusCode.BadRequest, false, new List<string> { "Invalid input." });
            return BadRequest(_response);
        }

        private APIResponse APIResponse(HttpStatusCode statusCode, 
                                        bool IsSuccess = true, 
                                        List<string> ErrorMessages = null,
                                        object Result = null)
        {
            var _response = new APIResponse();
            _response.StatusCode = statusCode;
            _response.IsSuccess = IsSuccess;
            _response.ErrorMessages = ErrorMessages;
            _response.Result = Result;

            return _response;
        }
    }
}
