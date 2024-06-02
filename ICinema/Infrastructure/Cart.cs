using ICinema.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ICinema.Infrastructure
{
    public class Cart
    {
        private readonly JsonSerializerOptions _serializerOptions = new() { WriteIndented = true };

        public List<Ticket> Tickets { get; set; } = [];

        public void RetrieveFromSession(HttpContext httpContext)
        {
            var sessionCart = httpContext.Session.GetString(nameof(Cart));
            if (sessionCart != null)
                Tickets = JsonSerializer.Deserialize<Cart>(sessionCart, _serializerOptions)?.Tickets ?? [];

            Tickets ??= [];
        }

        public void SaveToSession(HttpContext httpContext)
        {
            var serializedCart = JsonSerializer.Serialize(this, _serializerOptions);

            httpContext.Session.SetString(nameof(Cart), serializedCart);
        }
    }
}
