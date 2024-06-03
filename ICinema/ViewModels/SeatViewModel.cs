using ICinema.Models;

namespace ICinema.ViewModels
{
    public class SeatViewModel
    {
        public Seat Seat { get; set; } = new Seat();
        public decimal Price { get; set; }
        public string StyleType { get; set; } = string.Empty;
        public string StyleActive { get; set; } = string.Empty;
        public string StyleSelected { get; set; } = string.Empty;
    }
}
