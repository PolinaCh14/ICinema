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
                if (!_context.Users.ToList().Any(x => x.Email == model.Email) || !_context.Users.ToList().Any(x => x.PhoneNumber == model.PhoneNumber))
                {


                    await _context.Users.AddAsync(model.AddUser());

                    _context.SaveChanges();

                    //await Authenticate(user);
                    
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Такий користувач вже існує");
                }
            }
            return View(model);
        }

 

        //private async Task Authenticate(User user)
        //{
        //    var claims = new List<Claim>
        //    {
        //        new Claim("UserId", user.UserId.ToString()),
        //        new Claim(ClaimsIdentity.DefaultRoleClaimType, user.UserStatus),
        //        new Claim(ClaimsIdentity.DefaultNameClaimType, user.FirstName + " " + user.LastName),
        //    };

        //    ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

        //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        //}
    }
}