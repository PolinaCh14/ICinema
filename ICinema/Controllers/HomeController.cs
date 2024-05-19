using ICinema.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers
{
    public class HomeController(CinemaContext context) : Controller
    {
        private readonly CinemaContext _context = context;

        public IActionResult Index()
        {
            return View();
        }
    }
}
