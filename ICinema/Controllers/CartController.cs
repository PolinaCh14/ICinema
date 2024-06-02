using ICinema.Data;
using ICinema.Infrastructure;
using ICinema.Models;
using ICinema.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ICinema.Controllers;

public class CartController(CinemaContext context) : Controller
{
    private readonly CinemaContext _context = context;
    private readonly Cart _cart = new();

    public IActionResult Index()
    {
        _cart.RetrieveFromSession(HttpContext);

        var cartViewModel = new CartViewModel { Cart = _cart };

        if (_cart.Tickets.Count == 0)
            return View(cartViewModel);

        var session = _context.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Hall).ThenInclude(h => h.Technology)
                .First(s => s.SessionId == _cart.Tickets.First().SessionId);

        foreach (var ticket in _cart.Tickets)
            ticket.Seat = _context.Seats.First(s => s.SeatId == ticket.SeatId);

        cartViewModel.Session = session;

        return View(cartViewModel);
    }

    public IActionResult AddToCart(int sessionId, int seatId, decimal price)
    {
        var ticket = CreateTicket(sessionId, seatId, price);

        if (ticket == null) 
            return RedirectToAction("SeatsCatalog", "Sessions", new { sessionId });

        _cart.RetrieveFromSession(HttpContext);

        _cart.Tickets.Add(ticket);

        _cart.SaveToSession(HttpContext);

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
