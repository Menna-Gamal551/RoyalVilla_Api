using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RoyalVilla_Api.Data;
using RoyalVilla_Api.Data.Models;
using RoyalVilla_Api.Data.Models.DTO;

namespace RoyalVilla_Api.Recervice
{
    public class AuthService : IAuthService
    {
        private AppDbContext _db;
        private readonly IConfiguration _configuration;
        private IMapper _mapper;
        public AuthService(AppDbContext db, IMapper mapper,IConfiguration configuration)
        {
            _db = db;
            _mapper =mapper;
            _configuration = configuration;
           
        }
        
        public async Task<bool> isEmailExsitAsync(string email)
        {
           return await _db.Users.AnyAsync(e => e.Email.ToLower()==email.ToLower());
           
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(e => e.Email==loginRequestDTO.Email);
                if (user==null ||user.Password!=loginRequestDTO.Password)
                {
                    return null;
                }
                var token = GeneratejwtToken(user);
                return new LoginResponseDTO
                {
                    Token=token,
                    userDTO=_mapper.Map<UserDTO>(user)
                };
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
                User user = new()
                {
                    Name = registerationRequestDTO.Name,
                    Email = registerationRequestDTO.Email,
                    Password = registerationRequestDTO.Password,
                    Role = string.IsNullOrEmpty(registerationRequestDTO.Role) ? "Customer" : registerationRequestDTO.Role,
                    CreateDate= DateTime.Now,
                };
                await _db.AddAsync(user);
                await _db.SaveChangesAsync();
                return _mapper.Map<UserDTO>(user);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected error occured durin user registeration",ex);
            }
            
        }
        private string GeneratejwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("jwtsettings")["secret"]);
            var tokenDiscriptor = new SecurityTokenDescriptor
            {
                Subject=new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),

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
 