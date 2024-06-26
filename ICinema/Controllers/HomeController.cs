﻿using ICinema.Data;
using ICinema.Models;
using ICinema.ViewModels;
using ICinema.Infrastructure.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ICinema.Infrastructure;

namespace WebApp.Controllers
{
    public class HomeController(CinemaContext context) : Controller
    {
        private readonly CinemaContext _context = context;

        public IActionResult Index()
        {
            ViewBag.TicketsAmount = new Cart().TicketsAmount(HttpContext);

            var movies = _context.Movies.AsNoTracking().Include(m => m.Sessions).ThenInclude(s => s.Tickets).ToList();
            return View(movies);
        }

        [HttpGet]
        public async Task<IActionResult> MovieDetails(int id, DateIntervalEnum interval = DateIntervalEnum.Today)
        {
            ViewBag.TicketsAmount = new Cart().TicketsAmount(HttpContext);

            ArgumentOutOfRangeException.ThrowIfLessThan(id, 1, nameof(id));

            Movie? movie = await _context.Movies.FindAsync(id);
            if (movie is null)
            {
                return Redirect("/");
            }

            // to emulate current date as first date that has sessions in DB
            var currentDate = new DateOnly(2024, 7, 10);
            Expression<Func<Session, bool>> predicate;

            //condition to retrieve sessions of specified by user time interval (for today, tomorrow or whole week)
            if (interval == DateIntervalEnum.Tomorrow)
                predicate = s => s.Date == currentDate.AddDays((int)interval);
            else
                predicate = s => s.Date >= currentDate && s.Date <= currentDate.AddDays((int)interval);

            // retrive all sessions for movie that satisfies condition above with inner entities
            movie.Sessions = await _context.Sessions
                    .Where(s => s.MovieId == movie.MovieId)
                    .Where(predicate)
                    .Include(s => s.SessionType)
                    .Include(s => s.Hall)
                        .ThenInclude(h => h.Technology)
                    .ToListAsync();

            // initializing view model that has all info needed for schedule display (Movie, list of sessions, available dates etc.)
            var scheduleItem = new SessionScheduleViewModel()
            {
                Movie = movie,
                SessionDates = movie.Sessions.Select(s => s.Date).Distinct(),
                Technologies = movie.Sessions.Select(s => s.Hall.Technology).Distinct(),
                SessionsCinetech = movie.Sessions.Where(s => s.Hall.TechnologyId == ((int)TechnologyEnum.Cinetech)),
                SessionsIMAX = movie.Sessions.Where(s => s.Hall.TechnologyId == ((int)TechnologyEnum.IMAX)),
                Sessions4DX = movie.Sessions.Where(s => s.Hall.TechnologyId == ((int)TechnologyEnum._4DX)),
                CurrentDate = currentDate
            };

            // just to display date filter as selected (highlighted with white)
            int idx = interval == DateIntervalEnum.Week ? 2 : ((int)interval);
            scheduleItem.IntervalButtonClasses[idx] = "button-selected";

            return View(scheduleItem);
        }

        public List<Movie> MostPopularMovies()
        {
            return  _context.Movies
                .AsNoTracking()
                .Include(m => m.Sessions).ThenInclude(s => s.Tickets)
                .Where(m => m.Sessions.Any(s => s.Date >= DateOnly.FromDateTime(DateTime.Now)
                                                && s.Time > TimeOnly.FromDateTime(DateTime.Now)))
                .OrderByDescending(m => m.Sessions.SelectMany(s => s.Tickets).Count())
                .Take(4)
                .ToList();
        }

    }
}
