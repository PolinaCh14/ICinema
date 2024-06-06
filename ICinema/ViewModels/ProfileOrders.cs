using ICinema.Models;

namespace ICinema.ViewModels
{
    public class ProfileOrders
    {
        public int OrderId { get; set; }
        public string MovieName { get; set; }
        public DateOnly Date { get; set; }

        public TimeOnly Time { get; set; }
        public int HallId { get; set; }
        public string HallName { get; set; } = null!;
        public List<Ticket> Tickets { get; set; }
        public string OrderStatus { get; set; } 
        public decimal Price { get; set; }
    }
}
