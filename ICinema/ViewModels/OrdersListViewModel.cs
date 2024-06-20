using ICinema.Models;

namespace ICinema.ViewModels;

public class OrdersListViewModel
{
    public int? Id { get; set; }

    public List<Order> Items { get; set; } = [];

    public int PagesAmount { get; set; }

    public int CurrentPage { get; set; }

    public int StartIndex { get; set; }

    public int EndIndex { get; set; }
}
