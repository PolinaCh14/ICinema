using ICinema.Models;

namespace ICinema.ViewModels.HelperModels
{
    public class TicketModel
    {
        public int TicketId { get; set; }

        public decimal Price { get; set; }

        public int SeatId { get; set; }
        public SeatModel Seat { get; set; } = default!;

        public DateTime CreateDate { get; set; }

        public SessionModel Session { get; set; } = null!;

    }
}
