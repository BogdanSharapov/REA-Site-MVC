namespace REASite.ViewModel
{
    public class BookingEditViewModel
    {
        public int Id { get; set; }
        public int ApartmentId { get; set; }
        public string ApartmentTitle { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
