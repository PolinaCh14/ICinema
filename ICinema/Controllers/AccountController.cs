using ICinema.Data;
using ICinema.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ICinema.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace ICinema.Controllers
{
    public class AccountController(CinemaContext context) : Controller
    {
        private readonly CinemaContext _context = context;

        [HttpGet]
        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!_context.Users.Any(x => x.Email == model.Email || x.PhoneNumber == model.PhoneNumber))
                {
                    User user = model.ToRecord();
                    await _context.Users.AddAsync(user);
                    _context.SaveChanges();

                    await Authorize(user);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Такий користувач вже існує");
                }
            }
            return View(model);
        }

        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User? user = _context.Users.SingleOrDefault(x => x.Email == model.EmailOrPhone || x.PhoneNumber == model.EmailOrPhone);
                if (user != null && user.Password.Equals(model.Password))
                {
                    await Authorize(user);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Невірний email/номер телефону або пароль");
            }

            return View(model);
        }

        [Authorize]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


        private async Task Authorize(User user)
        {
            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new (ClaimTypes.Role, user.UserStatus),
                new (ClaimTypes.Email, user.Email),
                new (ClaimTypes.Name, user.FirstName + " " + user.LastName),
            };

            ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        }
    }
}