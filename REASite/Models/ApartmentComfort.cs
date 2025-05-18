using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REASite.Models
{
    public class ApartmentComfort
    {
        [Key]
        [Column(Order = 1)]
        public int ApartmentId { get; set; }

        [Key]
        [Column(Order = 2)]
        public ComfortEnum Comfort { get; set; }

        [ForeignKey("ApartmentId")]
        public Apartment Apartment { get; set; } = null!;
    }
}