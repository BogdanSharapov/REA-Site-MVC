using REASite.Areas.Identity.Data;

namespace REASite.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int ApartmentId { get; set; }
        public string UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "Pending";

        public Apartment Apartment { get; set; }
        public SiteUser User { get; set; }
    }
}
