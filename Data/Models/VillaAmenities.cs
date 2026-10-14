using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoyalVilla_Api.Data.Models
{
    public class VillaAmenities
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }
        public string? Description { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        //  [ForeignKey("villaId")]
        [ForeignKey(nameof(villa))]
        public int villaId { get; set; }
        public Villa? villa { get; set; }



    }
}
