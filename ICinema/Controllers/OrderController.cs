using ICinema.Data;
using ICinema.Infrastructure;
using ICinema.Infrastructure.Constants;
using ICinema.Models;
using ICinema.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Security.Claims;
using ICinema.Services.Interfaces;

namespace ICinema.Controllers
{
    public class OrderController(CinemaContext context, IEmailSender emailSender): Controller
    {
        private readonly CinemaContext _context = context;
        private readonly IEmailSender _emailSender = emailSender;

        public IActionResult MakeOrder()
        {
            ViewBag.TicketsAmount = new Cart().TicketsAmount(HttpContext);

            var cart = new Cart();
            cart.RetrieveFromSession(HttpContext);

            if (cart.Tickets.Count == 0 || !cart.Tickets.All(ticket => _context.Tickets.Contains(ticket)))
                return View("NoActiveTicketsErrorPage");

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
        public async Task<IActionResult> MakeOrder(ContactFormViewModel contactForm)
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
                return View("NoActiveTicketsErrorPage");

            if (contactForm.PaymentType == PaymentTypes.ONLINE && contactForm.PaymentCredentials == null)
            {
                contactForm.TotalPrice = decimal.Round(cart.Tickets.Sum(ticket => ticket.Price), 2);
                return View("OnlinePayment", contactForm);
            }
                    
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

            order.Tickets = [.. _context.Tickets
                .Include(t => t.Seat)
                .Include(t => t.Session).ThenInclude(t => t.Movie)
                .Include(t => t.Session).ThenInclude(t => t.Hall)
                .Where(o => o.OrderId == order.OrderId)];

            var emailMessage = CreateOrderEmailMessage(order);

            await _emailSender.SendEmail(order.UserEmail, $"Замовлення №{order.OrderId} оформлено!", emailMessage);

            ViewBag.TicketsAmount = new Cart().TicketsAmount(HttpContext);
            return View("OrderResult", order);
        }

        private string CreateOrderEmailMessage(Order order)
        {
            var ticketsList = "";

            foreach (var ticket in order.Tickets)
                ticketsList += $"\t{ticket.Seat.RowNumber} ряд    {ticket.Seat.SeatNumber} місце    {ticket.Price} грн\n";

            var emailMessage =
                $"Ваше замовлення №{order.OrderId} успішно оформлене!\n"
                + $"Фільм: {order.Tickets.First().Session.Movie.MovieName}.\n"
                + $"Дата: {order.Tickets.First().Session.Date}.\tЧас: {order.Tickets.First().Session.Time}.\tЗал: {order.Tickets.First().Session.Hall.HallName}.\n"
                + $"Квитки:\n"
                + ticketsList
                + $"Загалом {order.Price} грн.\n"
                + $"\nЗамовлення має бути викуплене до {order.Tickets.First().Session.Date} {order.Tickets.First().Session.Time.AddHours(-1)}! \n"
                ;

            return emailMessage;
        }

        public bool CheckPaymentTypeSelected(ContactFormViewModel contactForm) => 
            contactForm.PaymentType == PaymentTypes.CASH_DESK || contactForm.PaymentType == PaymentTypes.ONLINE;

        [Authorize(Roles = "Адміністратор")]
        public IActionResult ManageOrders(OrdersListViewModel model, int pageToDisplay = 1)
        {

            ViewBag.TicketsAmount = new Cart().TicketsAmount(HttpContext);

            IQueryable<Order> orders = _context.Orders
                .Include(o => o.Tickets).ThenInclude(t => t.Seat)
                .Include(o => o.Tickets).ThenInclude(t => t.Session).ThenInclude(s => s.Movie)
                .Include(o => o.Tickets).ThenInclude(t => t.Session).ThenInclude(s => s.Hall);

            if (model.Id.HasValue)
            {
                orders = orders.Where(x => x.OrderId == model.Id);
            }

            model.Items = orders.OrderByDescending(x => x.CreateDate).ToList();

            int ordersPerPage = 10;
            model.PagesAmount = (int)Math.Ceiling(model.Items.Count / (double)ordersPerPage);
            model.CurrentPage = pageToDisplay;
            model.StartIndex = ordersPerPage * (pageToDisplay - 1);
            model.EndIndex = model.StartIndex + ordersPerPage;

            return View(model);
        }

        [Authorize(Roles = "Адміністратор")]
        public IActionResult ProcessOrder(int id)
        {
            ViewBag.TicketsAmount = new Cart().TicketsAmount(HttpContext);

            var orders = _context.Orders
                .Include(o => o.Tickets).ThenInclude(t => t.Seat)
                .Include(o => o.Tickets).ThenInclude(t => t.Session).ThenInclude(s => s.Movie)
                .Include(o => o.Tickets).ThenInclude(t => t.Session).ThenInclude(s => s.Hall)
                .First(o => o.OrderId == id);

            return View(orders);
        }

        [HttpPost]
        [Authorize(Roles = "Адміністратор")]
        public IActionResult ProcessOrder(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();

            return RedirectToAction("ManageOrders");
        }
    }
}
