using REASite.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace REASite.Models
{
    public class Favorites
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ApartmentId { get; set; }
        public SiteUser User { get; set; }
        public Apartment Apartment { get; set; }
    }
}
