using ICinema.Models;

namespace ICinema.ViewModels
{
    public class SessionScheduleViewModel
    {
        public Movie Movie { get; set; } = new Movie();

        public IEnumerable<DateOnly> SessionDates { get; set; } = [];
        public IEnumerable<Technology> Technologies { get; set; } = [];

        public IEnumerable<Session>? SessionsCinetech { get; set; }
        public IEnumerable<Session>? SessionsIMAX { get; set; }
        public IEnumerable<Session>? Sessions4DX { get; set; }

        public DateOnly CurrentDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public string[] IntervalButtonClasses { get; set; } = ["", "", ""];
    }
}
