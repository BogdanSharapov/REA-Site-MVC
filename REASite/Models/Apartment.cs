﻿using System.Runtime.InteropServices;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace REASite.Models
{
    public class Apartment
    {
        [Key]
        [Column("apartment_id")]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        public int Price { get; set; }
        public byte Floor { get; set; }
        [Required]
        public ushort Area { get; set; }
        public byte RoomsCount { get; set; }
        public List<Favorites> Favorites { get; set; } = new List<Favorites>();
        public List<ApartmentImage> Images { get; set; } = new List<ApartmentImage>();
        public OfferTypeEnum OfferType { get; set; }

        [ForeignKey("Address")]
        public int AddressId { get; set; }
        [Required]
        public Address Address { get; set; } = new Address();
        public List<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<ApartmentComfort>? ApartmentComforts { get; set; } = new List<ApartmentComfort>();
    }
}
