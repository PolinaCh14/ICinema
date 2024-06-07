using ICinema.Models;

namespace ICinema.ViewModels.HelperModels
{
    public class MovieModel
    {
        public int MovieId { get; set; }

        public string MovieName { get; set; } = null!;

        public string Genres { get; set; } = null!;

        public string Directors { get; set; } = null!;

        public string Writers { get; set; } = null!;

        public string Cast { get; set; } = null!;

        public DateOnly ReleaseDate { get; set; }

        public int Duration { get; set; }

        public string Description { get; set; } = null!;

        public decimal IMDBRate { get; set; }

        public int AgeRestriction { get; set; }

        public string PosterPath { get; set; } = null!;

    }
}
