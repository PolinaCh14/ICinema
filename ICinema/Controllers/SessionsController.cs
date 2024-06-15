using ICinema.Data;
using ICinema.Infrastructure;
using ICinema.Infrastructure.Constants;
using ICinema.Infrastructure.Enums;
using ICinema.Models;
using ICinema.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ICinema.Controllers
{
    public class SessionsController(CinemaContext context) : Controller
    {
        private readonly CinemaContext _context = context;

        public async Task<IActionResult> Schedule(DateIntervalEnum interval = DateIntervalEnum.Today)
        {
            ViewBag.TicketsAmount = new Cart().TicketsAmount(HttpContext);

            var currentDate = new DateOnly(2024, 7, 10);
            Expression<Func<Session, bool>> predicate;

            if (interval == DateIntervalEnum.Tomorrow)
                predicate = s => s.Date == currentDate.AddDays((int)interval);
            else
                predicate = s => s.Date >= currentDate && s.Date <= currentDate.AddDays((int)interval);

            var scheduleItems = new List<SessionScheduleViewModel>();
            var movies = await _context.Movies
                .Include(m => m.Sessions)
                .Where(m => m.Sessions.Any(s => s.Date >= currentDate && s.Date <= currentDate.AddDays((int)interval)))
                .AsNoTracking()
                .ToListAsync();

            foreach (var movie in movies)
            {
                movie.Sessions = await _context.Sessions
                    .Where(s => s.MovieId == movie.MovieId)
                    .Where(predicate)
                    .Include(s => s.SessionType)
                    .Include(s => s.Hall)
                        .ThenInclude(h => h.Technology)
                    .AsNoTracking()
                    .ToListAsync();

                scheduleItems.Add(new SessionScheduleViewModel
                {
                    Movie = movie,
                    SessionDates = movie.Sessions.Select(s => s.Date).Distinct(),
                    Technologies = movie.Sessions.Select(s => s.Hall.Technology).Distinct(),
                    SessionsCinetech = movie.Sessions.Where(s => s.Hall.TechnologyId == ((int)TechnologyEnum.Cinetech)),
                    SessionsIMAX = movie.Sessions.Where(s => s.Hall.TechnologyId == ((int)TechnologyEnum.IMAX)),
                    Sessions4DX = movie.Sessions.Where(s => s.Hall.TechnologyId == ((int)TechnologyEnum._4DX)),
                    CurrentDate = currentDate
                });

                int idx = interval == DateIntervalEnum.Week ? 2 : ((int)interval);
                scheduleItems.Last().IntervalButtonClasses[idx] = "button-selected";
            }
            return View(scheduleItems);
        }

        public IActionResult SeatsCatalog(int sessionId)
        {
            ViewBag.TicketsAmount = new Cart().TicketsAmount(HttpContext);

            var cart = new Cart();
            cart.RetrieveFromSession(HttpContext);

            foreach (var ticket in cart.Tickets)
                ticket.Seat = _context.Seats.AsNoTracking().First(s => s.SeatId == ticket.SeatId);

            var session = _context.Sessions
                .Include(s => s.Movie)
                .Include(s => s.SessionType)
                .Include(s => s.Hall).ThenInclude(h => h.Technology)
                .Include(s => s.Hall).ThenInclude(h => h.Seats).ThenInclude(s => s.SeatType)
                .Include(s => s.Tickets).ThenInclude(t => t.Order)
                .FirstOrDefault(s => s.SessionId == sessionId);

            if (session is null)
                return RedirectToAction("Schedule");

            var seats = session.Hall.Seats.ToList();

            var rows = seats.Select(s => s.RowNumber).Distinct().ToList();

            var seatViewModels = new List<SeatViewModel>();

            foreach (var seat in seats)
            {
                bool isSeatInactive = session.Tickets
                    .Any(t => t.SeatId == seat.SeatId
                    && ((t.Order != null && t.Order.OrderStatus != OrderStatuses.CANCELED) || t.OrderId == null));

                var seatViewModel = new SeatViewModel
                {
                    Seat = seat,
                    Price = decimal.Round(seat.SeatType.BasePrice * session.Hall.Technology.Coefficient * session.SessionType.Coefficient, 2),
                    StyleType = seat.SeatType.SeatTypeId == (int)SeatTypeEnum.Default ? "button-seat-default" : "button-seat-vip",
                    StyleActive = isSeatInactive ? "button-seat-inactive inactive" : "",
                    StyleSelected = cart.Tickets.Exists(t => t.SeatId == seat.SeatId && t.SessionId == session.SessionId) ? "button-seat-selected inactive" : ""
                };

                seatViewModels.Add(seatViewModel);
            }

            var seatsCatalogViewModel = new SeatsCatalogViewModel
            {
                SeatViewModels = seatViewModels,
                Rows = rows,
                Session = session,
                Cart = cart
            };

            return View(seatsCatalogViewModel);
        }
    }
}