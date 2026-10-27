using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RoyalVilla_Api.Data;
using RoyalVilla_Api.Data.Models;
using RoyalVillaDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Microsoft.VisualBasic;

namespace RoyalVilla_Api.Recervice
{
    public class AuthService : IAuthService
    {
        private AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private IMapper _mapper;
        public AuthService(AppDbContext db, IMapper mapper, IConfiguration configuration, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _mapper =mapper;
            _configuration = configuration;
            _roleManager=roleManager;
            _userManager=userManager;
        }

        public async Task<bool> isEmailExsitAsync(string email)
        {
           return await _db.ApplicationUsers.AnyAsync(e => e.Email.ToLower()==email.ToLower());
           
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            try
            {
                var user = await _db.ApplicationUsers.FirstOrDefaultAsync(e => e.Email==loginRequestDTO.Email);
                if (user==null )
                {
                    return null;
                }
                var isValid=await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
                if (!isValid)
                {
                    return null;
                }
                 var token = await  GeneratejwtToken(user);
                var roles  = await  _userManager.GetRolesAsync(user);
                LoginResponseDTO loginResponseDTo= new LoginResponseDTO
                {
                    Token = token,
                    userDTO = _mapper.Map<UserDTO>(user)
                };
                loginResponseDTo.userDTO.Role=roles.FirstOrDefault()??"Customer";

                return loginResponseDTo;

            }
            catch (Exception ex)
            { 

                throw new InvalidOperationException($"Unexpected error occured durin user login", ex);
            }
        }

        public async Task<UserDTO?> RegisterAsync(RegisterationRequestDTO registerationRequestDTO)
        {
            try
            {


                if (await isEmailExsitAsync(registerationRequestDTO.Email))
                {
                    throw new InvalidOperationException($"email with {registerationRequestDTO.Email} is already exsit!");
                }
                ApplicationUser user = new()
                {
                    Name = registerationRequestDTO.Name,
                    Email = registerationRequestDTO.Email,
                    NormalizedUserName=registerationRequestDTO.Email.ToUpper(),
                    UserName=registerationRequestDTO.Email,
                    EmailConfirmed=true
                };
                var result= await _userManager.CreateAsync(user,registerationRequestDTO.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(" , ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"User registeration failed : {errors}");
                }
                var role = string.IsNullOrEmpty( registerationRequestDTO.Role) ? "Customer" : registerationRequestDTO.Role;
                if(!await _roleManager.RoleExistsAsync(role))
                { 
                 await _roleManager.CreateAsync(new IdentityRole(role));
                }
                await _userManager.AddToRoleAsync(user, role);
                var userDto = _mapper.Map<UserDTO>(user);
                userDto.Role= role;
                return userDto;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected error occured durin user registeration",ex);
            }
            
        }
        private async Task<string> GeneratejwtToken(ApplicationUser user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("jwtsettings")["secret"]);
            var roles=await _userManager.GetRolesAsync(user);
            var tokenDiscriptor = new SecurityTokenDescriptor
            {
                Subject=new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),

                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials=new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDiscriptor);
            return tokenHandler.WriteToken(token);
        }



    }
}
 