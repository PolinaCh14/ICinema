using ICinema.Data;
using ICinema.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers
{
    public class HomeController(CinemaContext context) : Controller
    {
        private readonly CinemaContext _context = context;

        public IActionResult Index()
        {
            var movies = _context.Movies.AsNoTracking().ToList();
            return View(movies);
        }

        [HttpGet]
        public IActionResult MovieDetails(int id)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(id, 1, nameof(id));

            Movie? movie = _context.Movies.Find(id);
            if (movie is not null)
            {
                return View(movie);
            }

            return Redirect("/");
        }
    }
}
