using ICinema.Data;
using ICinema.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ICinema.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Google;
using ICinema.Infrastructure.Constants;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ICinema.Infrastructure;

namespace ICinema.Controllers
{
    public class AccountController(CinemaContext context) : Controller
    {
        private readonly CinemaContext _context = context;

        [HttpGet]
        public ActionResult Register()
        {
            ViewBag.IsCartEmpty = new Cart().IsEmpty(HttpContext);
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!_context.Users.Any(
                    x => x.Email == model.Email
                    || (x.PhoneNumber != null && x.PhoneNumber == model.PhoneNumber)))
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

            ViewBag.IsCartEmpty = new Cart().IsEmpty(HttpContext);
            return View(model);
        }

        public ActionResult Login()
        {
            ViewBag.IsCartEmpty = new Cart().IsEmpty(HttpContext);
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User? user = _context.Users.SingleOrDefault(
                    x => x.Email == model.EmailOrPhone
                    || (x.PhoneNumber != null && x.PhoneNumber == model.EmailOrPhone));
                if (user != null && user.Password.Equals(model.Password))
                {
                    await Authorize(user);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Невірний email/номер телефону або пароль");
            }

            ViewBag.IsCartEmpty = new Cart().IsEmpty(HttpContext);
            return View(model);
        }

        [Authorize]
        public async Task<ActionResult> Logout()
        {
            ViewBag.IsCartEmpty = new Cart().IsEmpty(HttpContext);
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

        public ActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("~/signin-google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
                return RedirectToAction("Schedule", "Sessions");

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;

            if (claims == null)
                return RedirectToAction("Register");

            var firstName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value ?? "";
            var lastName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value ?? "";
            var email = claims.First(c => c.Type == ClaimTypes.Email).Value;
            var phoneNumber = claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone)?.Value ?? null;

            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    Password = "",
                    UserStatus = UserStatuses.CLIENT
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            await Authorize(user);

            ViewBag.IsCartEmpty = new Cart().IsEmpty(HttpContext);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> MyProfile()
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User? user = _context.Users.Find(userId);

            if (user == null)
            {
                throw new Exception("Unknown user");
            }

            var orders = _context.Orders.Include(x => x.User)
                                        .Include(x => x.Tickets).ThenInclude(x => x.Session)
                                                                          .ThenInclude(x => x.Hall)
                                        .Include(x => x.Tickets).ThenInclude(x => x.Session)
                                                                          .ThenInclude(x => x.Movie)
                            .OrderBy(x=>x.CreateDate).ToList();

            ProfileViewModel model = new ProfileViewModel(user);
            model.Orders = orders;

            return View();
            ViewBag.IsCartEmpty = new Cart().IsEmpty(HttpContext);
            return View(new ProfileViewModel(user));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MyProfile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (!_context.Users.Any(
                    x => ((x.Email == model.Email)
                    || (x.PhoneNumber != null && x.PhoneNumber == model.PhoneNumber))
                    && x.UserId != userId))
                {

                    User? user = await _context.Users.SingleOrDefaultAsync(x => x.UserId == userId);

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;

                    _context.SaveChanges();
                    model.IsEditMode = false;

                    await Authorize(user);

                    return RedirectToAction(nameof(MyProfile), model);
                }
                else
                {
                    ModelState.AddModelError("", "Такий користувач вже існує");
                }
            }

            model.IsEditMode = true;
            ViewBag.IsCartEmpty = new Cart().IsEmpty(HttpContext);
            return View(model);
        }
    }
}