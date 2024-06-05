using ICinema.Models;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace ICinema.Infrastructure
{
    public class Cart
    {
        private static readonly JsonSerializerOptions _serializerOptions = new() { WriteIndented = true };

        public List<Ticket> Tickets { get; set; } = [];

        public void RetrieveFromSession(HttpContext httpContext)
        {
            var sessionCart = httpContext.Session.GetString(nameof(Cart));
            if (sessionCart != null)
                Tickets = DeserializeCart(sessionCart)?.Tickets ?? [];

            Tickets ??= [];
        }

        public void SaveToSession(HttpContext httpContext)
        {
            var serializedCart = SerializeCart();

            httpContext.Session.SetString(nameof(Cart), serializedCart);
        }

        public string SerializeCart() => JsonSerializer.Serialize(this, _serializerOptions);

        public static Cart? DeserializeCart(string cartJson) => JsonSerializer.Deserialize<Cart>(cartJson, _serializerOptions);

        public bool IsEmpty(HttpContext httpContext) 
        {
            RetrieveFromSession(httpContext);
            return !Tickets.Any(t => t.TicketId != 0);
        } 
    }
}
