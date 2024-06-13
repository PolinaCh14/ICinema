using ICinema.Models;

namespace ICinema.ViewModels.HelperModels
{
    public class HallModel
    {
        public int HallId { get; set; }

        public string HallName { get; set; } = null!;

        //public ICollection<SeatModel> Seats { get; set; } = new List<SeatModel>();

        public Technology Technology { get; set; } = null!;
    }
}
