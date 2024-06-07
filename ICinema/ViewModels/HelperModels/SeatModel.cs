using ICinema.Models;

namespace ICinema.ViewModels.HelperModels
{
    public class SeatModel
    {
        public int SeatId { get; set; }

        public int RowNumber { get; set; }

        public int SeatNumber { get; set; }

        public virtual SeatType SeatType { get; set; } = null!;
    }
}
