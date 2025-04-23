using System.Runtime.InteropServices;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace REASite.Models
{
    public class ApartmentModel
    {
        [Key]
        [Column("apartment_id")]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public Addres Addres { get; set; }
        [Required]
        public ushort Area { get; set; }
        public byte RoomsCount { get; set; }
        public string OfferType { get; set; } = string.Empty;
        public bool isFavorite { get; set; }
        public string imgURL { get; set; } = string.Empty;
    }
    public class Addres
    {
        [Key]
        [Required]
        public string HouseIndex { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string HouseNum { get; set; }
        //[ForeignKey]
        public string ApartmentID { get; set; }
        public List<ApartmentModel>? Apartment { get; set; }
    }
}
