using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoyalVillaDTO;
using RoyalVillaWeb.Services;
using RoyalVillaWeb.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RoyalVillaWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService ,IMapper mapper)
        {
          _authService = authService;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO)
        {
          
            try
            {
                var response = await _authService.LoginAsync<ApiResponse<LoginResponseDTO>>(loginRequestDTO);
                if (response!=null&&response.Success&&response.Data!=null)
                {
                    LoginResponseDTO loginResponseDTO = response.Data;
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(loginResponseDTO.Token);
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type=="email").Value));
                    identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type=="role").Value));
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal);
                    HttpContext.Session.SetString(SD.SessionToken, loginResponseDTO.Token);
                    return RedirectToAction("Index","Home");
                }
            }
            catch (Exception ex)
            {
                TempData["error"]=$"an error occured :{ex.Message}";
            }

            return View();
        }

        public IActionResult Register()
        {
            return View(new RegisterationRequestDTO { 
            Name=string.Empty,
            Email=string.Empty,
            Password=string.Empty,
            Role="Customer"
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterationRequestDTO registerationRequestDTO)
        {

            try
            {
                ApiResponse<UserDTO> response = await _authService.RegisterAsync<ApiResponse<UserDTO>>(registerationRequestDTO);
                if (response!=null&&response.Success&&response.Data!=null)
                {
                    TempData["success"]="Registeration Successful! please login with your credentials.";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["error"]=response?.Message??"Registeration Failed. please try again";
                    return View(registerationRequestDTO);
                }
            }
            catch (Exception ex)
            {
                TempData["error"]=$"an error occured :{ex.Message}";
            }

            return View(registerationRequestDTO);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        public async Task< IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index","Home");
        }

    }
}
