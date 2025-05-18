using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REASite.Models
{
    public class ApartmentImage
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Apartment")]
        public int ApartmentId { get; set; }
        public Apartment Apartment { get; set; } = null!;
        [Required]
        public string ImageURL { get; set; } = string.Empty;
    }
}