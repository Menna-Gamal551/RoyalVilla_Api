using System.ComponentModel.DataAnnotations;

namespace RoyalVillaDTO
{
    public class UserDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string Role { get; set; } = default!;
    }
}
