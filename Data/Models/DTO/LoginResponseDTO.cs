namespace RoyalVilla_Api.Data.Models.DTO
{
    public class LoginResponseDTO
    {
        public string? Token { get; set; }    
        public UserDTO? userDTO { get; set; }    
    }
}
