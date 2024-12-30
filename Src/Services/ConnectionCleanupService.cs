using dating_course_api.Src.Data;
using Microsoft.EntityFrameworkCore;

namespace dating_course_api.Src.Services
{
    public class ConnectionCleanupService(
        IServiceProvider services,
        ILogger<ConnectionCleanupService> logger
    ) : IHostedService, IDisposable
    {
        private readonly IServiceProvider _services = services;
        private readonly ILogger<ConnectionCleanupService> _logger = logger;
        private Timer? _timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Connection Cleanup Service starting at: {time}",
                DateTime.UtcNow
            );

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            try
            {
                using var scope = _services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();

                var cutoffTime = DateTime.UtcNow.AddMinutes(-5);

                var staleConnections = await context
                    .Connections.Where(c => !c.Connected && c.ConnectedAt < cutoffTime)
                    .ToListAsync();

                if (0 < staleConnections.Count)
                {
                    _logger.LogInformation(
                        "Removing {count} stale connections at {time}",
                        staleConnections.Count,
                        DateTime.UtcNow
                    );

                    context.Connections.RemoveRange(staleConnections);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while cleaning up connections at {time}",
                    DateTime.UtcNow
                );
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Connection Cleanup Service stopping at: {time}",
                DateTime.UtcNow
            );

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
