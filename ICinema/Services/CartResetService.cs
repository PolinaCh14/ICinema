using ICinema.Data;
using ICinema.Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;

namespace ICinema.Services;

public class CartResetService(IServiceScopeFactory serviceScopeFactory) : IHostedService, IDisposable
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private Timer _timer = null!;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<CinemaContext>();

        var bookedTickets = dbContext.Tickets.Where(t => t.OrderId == null).ToList();
        var ticketsToDelete = bookedTickets.Where(t => DateTime.Now.Subtract(t.CreateDate).TotalMinutes >= 15).ToList();

        dbContext.Tickets.RemoveRange(ticketsToDelete);
        dbContext.SaveChanges();

        var currentDate = DateOnly.FromDateTime(DateTime.Now);
        var currenTime = TimeOnly.FromDateTime(DateTime.Now);
        var nowPlusOneHour = TimeOnly.FromDateTime(DateTime.Now.AddHours(1));


        var orders = dbContext.Orders
            .Include(o => o.Tickets).ThenInclude(t => t.Session)
            .Where(o => (o.OrderStatus == OrderStatuses.NEW || o.OrderStatus == OrderStatuses.PAID)
                && o.Tickets.First().Session.Date == currentDate)
            .Select(x => new { Order = x, SessionTime = x.Tickets.First().Session.Time })
            .ToList();

        foreach (var order in orders)
        {
            if (currenTime >= order.SessionTime && order.Order.OrderStatus == OrderStatuses.PAID)
                order.Order.OrderStatus = OrderStatuses.COMPLETED;

            if (nowPlusOneHour >= order.SessionTime && order.Order.OrderStatus == OrderStatuses.NEW)
                order.Order.OrderStatus = OrderStatuses.CANCELED;
        }
            
        dbContext.SaveChanges();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose() => _timer?.Dispose();
}