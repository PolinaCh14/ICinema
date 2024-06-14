using ICinema.Models;

namespace ICinema.ViewModels;

public class OrdersListViewModel
{
    public int? Id { get; set; }

    public List<Order> Items { get; set; } = [];
}
