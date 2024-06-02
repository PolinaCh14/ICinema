using ICinema.Models;
using System.Text.Json;

namespace ICinema.Infrastructure
{
    public class Cart
    {
        private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions { WriteIndented = true };
        public List<Ticket> CartTickets { get; set; } = [];
    }
}
