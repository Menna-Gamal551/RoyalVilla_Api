using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RoyalVilla_Api.Data.Models
{
    public class VillaAmentyCreateDTO
    {

        [Required]
        [MaxLength(100)]
        public required  string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public int villaId { get; set; }
       
    }
}
