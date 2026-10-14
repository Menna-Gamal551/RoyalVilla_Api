using System.ComponentModel.DataAnnotations;

namespace RoyalVilla_Api.Data.Models
{
    public class Villa
    {
        [Key]
        public  int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        public string ?Details { get; set; }
        public int Sqft { get; set; }
        public double Rate { get; set; }
        public int Occupancy { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        //public List<VillaAmenities>villaAmenities { get; set; }
        public ICollection<VillaAmenities>?Amenities { get; set; }

    }
}
