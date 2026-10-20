using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoyalVillaDTO;
using RoyalVilla_Api.Data;
using RoyalVilla_Api.Recervice;
using Microsoft.IdentityModel.Tokens;

namespace RoyalVilla_Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService=authService;
       
        [HttpPost("Register")]
        
        [ProducesResponseType(typeof(ApiResponse<UserDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        
        public async Task <ActionResult<ApiResponse<UserDTO>>> Register( RegisterationRequestDTO registerRequestDTO)
        {
            try 
            {
                if (registerRequestDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Registeration Data is Required"));
                }
                if (await _authService.isEmailExsitAsync(registerRequestDTO.Email))
                {
                    return Conflict(ApiResponse<object>.Conflict($"email with {registerRequestDTO.Email} is already exsit!"));
                }

                var userDTO = await _authService.RegisterAsync(registerRequestDTO);
                if (userDTO==null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Registeration Failed"));
                }


                var response = ApiResponse<UserDTO>.CreatedAt("User registered Successfully", userDTO);
                return CreatedAtAction(nameof(Register), response);
            }
            catch (Exception ex)
            {
                var errorresponse = ApiResponse<Object>.Error(500, "An error occured while Registeration  ", ex.Message);

                return StatusCode(500, errorresponse);
            }


        }
        [HttpPost("Login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<ApiResponse<LoginResponseDTO>>> Login(LoginRequestDTO loginRequestDTO)
        {
            try
            {
                if (loginRequestDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Login Data is Required"));
                }
                

                var loginResponse = await _authService.LoginAsync(loginRequestDTO);
                if (loginResponse==null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Login Failed"));
                }


                var response = ApiResponse<LoginResponseDTO>.Ok("User Login Successfully", loginResponse);
                return Ok( response);
            }
            catch (Exception ex)
            {
                var errorresponse = ApiResponse<Object>.Error(500, "An error occured while Login  ", ex.Message);

                return StatusCode(500, errorresponse);
            }


        }
    }
}
