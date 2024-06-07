using ICinema.Models;

namespace ICinema.ViewModels.HelperModels
{
    public class SessionModel
    {
        public int SessionId { get; set; }

        public int SessionTypeId { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly Time { get; set; }

        public string Format { get; set; } = null!;

        public HallModel Hall { get; set; } = null!;

        public MovieModel Movie { get; set; } = null!;

        public virtual SessionType SessionType { get; set; } = null!;
    }
}
