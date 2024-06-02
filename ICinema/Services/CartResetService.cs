using ICinema.Controllers;

namespace ICinema.Services
{
    public class CartResetService(IServiceProvider serviceProvider) : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private Timer _timer = null!;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            using var scope = _serviceProvider.CreateScope();

            var myController = scope.ServiceProvider.GetRequiredService<CartController>();
            myController.ClearCart();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
