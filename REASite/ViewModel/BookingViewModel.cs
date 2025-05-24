namespace REASite.ViewModel
{
    public class BookingViewModel
    {
        public int Id { get; set; }
        public int ApartmentId { get; set; }
        public string ApartmentTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
    }
}
