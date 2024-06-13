using ICinema.Models;

namespace ICinema.ViewModels.HelperModels
{
    public class OrdersModel
    {
        public int OrderId { get; set; }
        public string MovieName { get; set; } = null!;
        public DateOnly Date { get; set; }

        public TimeOnly Time { get; set; }
        public List<TicketModel> Tickets { get; set; } = [];
        public string OrderStatus { get; set; } = null!;
        public decimal Price { get; set; }
    }
}
