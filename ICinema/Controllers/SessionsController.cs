using ICinema.Data;
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
                .ToListAsync();

            foreach (var movie in movies)
            {
                movie.Sessions = await _context.Sessions
                    .Where(s => s.MovieId == movie.MovieId)
                    .Where(predicate)
                    .Include(s => s.SessionType)
                    .Include(s => s.Hall)
                        .ThenInclude(h => h.Technology)
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

    }
}
