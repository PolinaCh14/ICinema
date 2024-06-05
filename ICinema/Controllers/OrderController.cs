
using ICinema.Data;
using ICinema.Infrastructure;
using ICinema.Infrastructure.Constants;
using ICinema.Models;
using ICinema.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ICinema.Controllers
{
    public class OrderController(CinemaContext context): Controller
    {
        private readonly CinemaContext _context = context;

        public IActionResult MakeOrder()
        {
            var cart = new Cart();
            cart.RetrieveFromSession(HttpContext);

            if (cart.Tickets.Count == 0 || !cart.Tickets.All(ticket => _context.Tickets.Contains(ticket)))
                //TODO: redirection to Error Page "Sorry, u don't have active tickets in cart. Please, select new tickets"
                return RedirectToAction("Index", "Cart");

            if (User?.Identity?.IsAuthenticated != true)
                return View();

            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _context.Users.Find(userId);

            if (user != null)
                return View(new ContactFormViewModel
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                });

            return View();
        }

        [HttpPost]
        public IActionResult MakeOrder(ContactFormViewModel contactForm)
        {
            if (!ModelState.IsValid || !CheckPaymentTypeSelected(contactForm)) 
            {
                if (!CheckPaymentTypeSelected(contactForm)) 
                    ModelState.AddModelError("PaymentType", "Оберіть тип оплати");

                return View(contactForm);
            }

            var cart = new Cart();
            cart.RetrieveFromSession(HttpContext);

            if (cart.Tickets.Count == 0 || !cart.Tickets.All(ticket => _context.Tickets.Contains(ticket)))
                //TODO: redirection to Error Page "Sorry, u don't have active tickets in cart. Please, select new tickets"
                return RedirectToAction("Index", "Cart");

            var order = new Order
            {
                Price = cart.Tickets.Sum(x => x.Price),
                UserFirstName = contactForm.FirstName,
                UserLastName = contactForm.LastName,
                UserEmail = contactForm.Email,
                UserId = contactForm.UserId,
                PaymentType = contactForm.PaymentType,
                PaymentDate = contactForm.PaymentType == PaymentTypes.ONLINE ? DateTime.Now : null,
                OrderStatus = contactForm.PaymentType == PaymentTypes.ONLINE ? OrderStatuses.PAID : OrderStatuses.NEW,
                PaymentCredentials = contactForm.PaymentCredentials
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var ticket in cart.Tickets)
            {
                ticket.OrderId = order.OrderId;
                _context.Tickets.Update(ticket);
            }
                
            _context.SaveChanges();

            cart.Tickets = [];
            cart.SaveToSession(HttpContext);

            return RedirectToAction("Index", "Home");
        }

        public bool CheckPaymentTypeSelected(ContactFormViewModel contactForm) => 
            contactForm.PaymentType == PaymentTypes.CASH_DESK || contactForm.PaymentType == PaymentTypes.ONLINE;
    }
}
