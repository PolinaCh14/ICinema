using ICinema.Models;

namespace ICinema.ViewModels
{
    public class SeatViewModel
    {
        public Seat Seat { get; set; } = new Seat();
        public string StyleType { get; set; } = string.Empty;
        public string StyleActive { get; set; } = string.Empty;
    }
}
