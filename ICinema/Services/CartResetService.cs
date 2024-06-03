using ICinema.Data;

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
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose() => _timer?.Dispose();
}