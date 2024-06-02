using ICinema.Infrastructure;
using ICinema.Models;

namespace ICinema.ViewModels;

public class CartViewModel
{
    public Cart Cart { get; set; } = null!;
    public Session Session { get; set; } = null!;
}
