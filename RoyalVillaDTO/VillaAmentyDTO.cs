using System.ComponentModel.DataAnnotations;

namespace RoyalVillaDTO
{
    public class VillaAmentyDTO
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public int villaId { get; set; }
        public string VillaName { get; set; }
    }
}
