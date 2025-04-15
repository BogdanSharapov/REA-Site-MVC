using System.Runtime.InteropServices;
using System.ComponentModel.DataAnnotations;

namespace REASite.Models
{
    public class ApartmentModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        [Required]
        public Addres Addres { get; set; }
        [Required]
        public ushort Area { get; set; }
        public byte RoomsCount { get; set; }
        public string OfferType { get; set; }
        public bool isFavorite { get; set; }
        public string imgURL { get; set; }
    }
    public class Addres
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNum { get; set; }
    }
}
