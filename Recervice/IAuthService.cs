using RoyalVillaDTO;

namespace RoyalVilla_Api.Recervice
{
    public interface IAuthService
        {
        Task<UserDTO?>RegisterAsync(RegisterationRequestDTO registerationRequestDTO);
        Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO);
        Task<bool>isEmailExsitAsync(string email);

        }
}
