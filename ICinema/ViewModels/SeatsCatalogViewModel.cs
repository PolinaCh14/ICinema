using ICinema.Models;

namespace ICinema.ViewModels
{
    public class SeatsCatalogViewModel
    {
        public List<SeatViewModel> SeatViewModels { get; set; } = [];
        public List<int> Rows { get; set; } = [];
        public Session Session { get; set; } = new();
    }
}
