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
    private Timer _timer = null!;

    public IActionResult Index()
    {
        ViewBag.IsCartEmpty = new Cart().IsEmpty(HttpContext);

        _cart.RetrieveFromSession(HttpContext);

        foreach (var ticket in _cart.Tickets)
            if (!_context.Tickets.Any(t => t.SeatId == ticket.SeatId && t.SessionId == ticket.SessionId))
                return View(new CartViewModel { Cart = new() });

        var cartViewModel = new CartViewModel { Cart = _cart };

        if (_cart.Tickets.Count == 0)
            return View(cartViewModel);

        foreach (var ticket in _cart.Tickets)
            ticket.Seat = _context.Seats.First(s => s.SeatId == ticket.SeatId);

        var session = _context.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Hall).ThenInclude(h => h.Technology)
                .First(s => s.SessionId == _cart.Tickets.First().SessionId);

        cartViewModel.Session = session;

        return View(cartViewModel);
    }

    public IActionResult SelectTicket(int sessionId, int seatId, decimal price)
    {
        ViewBag.IsCartEmpty = new Cart().IsEmpty(HttpContext);

        var ticket = CreateTicket(sessionId, seatId, price);

        if (ticket == null)
            return RedirectToAction("SeatsCatalog", "Sessions", new { sessionId });

        _cart.RetrieveFromSession(HttpContext);

        if (_cart.Tickets.Any(t => t.SessionId != sessionId))
        {
            var differentSessionViewModel = new DifferentSessionViewModel
            {
                PreviousSession = sessionId,
                SelectedSession = _cart.Tickets.First().SessionId
            };
            return View("DifferentSessionError", differentSessionViewModel);
        }
            
        _cart.Tickets.Add(ticket);

        _cart.SaveToSession(HttpContext);

        return RedirectToAction("SeatsCatalog", "Sessions", new { sessionId });
    }

    public IActionResult RemoveTicket(int sessionId, int seatId)
    {
        _cart.RetrieveFromSession(HttpContext);

        _cart.Tickets.RemoveAll(t => t.SessionId == sessionId && t.SeatId == seatId);

        _cart.SaveToSession(HttpContext);

        ViewBag.IsCartEmpty = _cart.IsEmpty(HttpContext);
        return RedirectToAction("SeatsCatalog", "Sessions", new { sessionId });
    }

    public IActionResult DeleteTicket(int ticketId)
    {
        _cart.RetrieveFromSession(HttpContext);

        _context.Tickets.Remove(_cart.Tickets.First(t => t.TicketId == ticketId));
        _context.SaveChanges();

        _cart.Tickets.RemoveAll(t => t.TicketId == ticketId);

        _cart.SaveToSession(HttpContext);

        ViewBag.IsCartEmpty = _cart.IsEmpty(HttpContext);
        return RedirectToAction("Index");
    }

    public IActionResult AddToCart()
    {
        ViewBag.IsCartEmpty = new Cart().IsEmpty(HttpContext);

        _cart.RetrieveFromSession(HttpContext);

        try
        {
            foreach (var ticket in _cart.Tickets)
                if (!_context.Tickets.Any(t => t.SeatId == ticket.SeatId && t.SessionId == ticket.SessionId))
                    _context.Tickets.Add(ticket);

            _context.SaveChanges();
        }
        catch (Exception)
        {
            _cart.Tickets = [];
            _cart.SaveToSession(HttpContext);
            return View("NoActiveTicketsErrorPage");
        }

        var lastCreateDate = _cart.Tickets.Last().CreateDate;

        foreach (var ticket in _cart.Tickets)
        {
            if (ticket.CreateDate != lastCreateDate)
                ResetTicketDateTime(ticket.TicketId, lastCreateDate);
        }

        _cart.SaveToSession(HttpContext);

        return RedirectToAction(nameof(Index));
    }

    private Ticket? CreateTicket(int sessionId, int seatId, decimal price)
    {
        if (_context.Tickets.Any(t => t.SessionId == sessionId && t.SeatId == seatId))
            return null;

        var ticket = new Ticket
        {
            Price = price,
            SessionId = sessionId,
            SeatId = seatId,
        };

        return ticket;
    }

    private void ResetTicketDateTime(int ticketId, DateTime newCreateDate)
    {
        var ticket = _context.Tickets.Find(ticketId);
        if (ticket != null)
        {
            ticket.CreateDate = newCreateDate;
            _context.SaveChanges();
        }
    }
}
