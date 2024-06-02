using ICinema.Data;
using ICinema.Infrastructure;
using ICinema.Models;
using ICinema.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ICinema.Controllers;

public class CartController(CinemaContext context) : Controller
{
    private readonly CinemaContext _context = context;
    private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions { WriteIndented = true };

    private Cart _cart = null!;

    public void RetrieveFromSession()
    {
        var sessionCart = HttpContext.Session.GetString(nameof(Cart));
        if (sessionCart != null)
            _cart = JsonSerializer.Deserialize<Cart>(sessionCart, _serializerOptions)!;

        _cart ??= new();
    }

    public void SaveToSession()
    {
        var serializedCart = JsonSerializer.Serialize(_cart, _serializerOptions);

        HttpContext.Session.SetString(nameof(Cart), serializedCart);
    }

    public IActionResult Index()
    {
        RetrieveFromSession();

        var cartViewModel = new CartViewModel { Cart = _cart };

        if (_cart.CartTickets.Count == 0)
            return View(cartViewModel);

        var session = _context.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Hall).ThenInclude(h => h.Technology)
                .First(s => s.SessionId == _cart.CartTickets.First().SessionId);

        foreach (var ticket in _cart.CartTickets)
            ticket.Seat = _context.Seats.First(s => s.SeatId == ticket.SeatId);

        cartViewModel.Session = session;

        return View(cartViewModel);
    }

    public IActionResult AddToCart(int sessionId, int seatId, decimal price)
    {
        var ticket = CreateTicket(sessionId, seatId, price);

        if (ticket == null) 
            return RedirectToAction("SeatsCatalog", "Sessions", new { sessionId });

        RetrieveFromSession();

        _cart.CartTickets.Add(ticket);

        SaveToSession();

        return RedirectToAction("SeatsCatalog", "Sessions", new { sessionId });
    }

    private Ticket? CreateTicket(int sessionId, int seatId, decimal price)
    {
        if (_context.Tickets.Any(t => t.SessionId == sessionId && t.SeatId == seatId))
            return null;

        var ticket = new Ticket
        {
            Price = price,
            SessionId = sessionId,
            SeatId = seatId
        };

        _context.Tickets.Add(ticket);
        _context.SaveChanges();

        return ticket;
    }

    public void ClearCart()
    {
        Console.WriteLine("Clearing cart every minute...");
    }
}
