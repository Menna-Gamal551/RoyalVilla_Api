using System.ComponentModel.DataAnnotations;

namespace RoyalVilla_Api.Data.Models.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string Role { get; set; } = default!;
    }
}
