using System.ComponentModel.DataAnnotations;

namespace REASite.Models
{
    public class Address
    {
            [Key]
            public int Id { get; set; }
            [Required]
            public string Country { get; set; }
            [Required]
            public string City { get; set; }
            [Required]
            public string Street { get; set; }
            [Required]
            public string HouseNum { get; set; }
            public ICollection<Apartment>? Apartments { get; set; }
    }
}
