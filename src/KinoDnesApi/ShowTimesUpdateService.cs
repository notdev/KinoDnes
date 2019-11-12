using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KinoDnesApi.DataProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sentry;

namespace KinoDnesApi
{
    public class ShowTimesUpdateService : IHostedService, IDisposable
    {
        private readonly IShowTimesProvider _showTimesProvider;
        private readonly ICsfdDataProvider _csfdDataProvider;
        private readonly ILogger<ShowTimesUpdateService> _logger;
        private readonly ISentryClient _sentryClient;
        private Timer _timer;
        private static readonly object Lock = new object();

        public ShowTimesUpdateService(IShowTimesProvider showTimesProvider, ICsfdDataProvider csfdDataProvider,
            ILogger<ShowTimesUpdateService> logger, IServiceProvider serviceProvider)
        {
            _showTimesProvider = showTimesProvider;
            _csfdDataProvider = csfdDataProvider;
            _logger = logger;
            try
            {
                _sentryClient = (ISentryClient) serviceProvider.GetService(typeof(ISentryClient));
            }
            catch
            {
                _sentryClient = null;
            }
        }

        private void Update(object state)
        {
            try
            {
                lock (Lock)
                {
                    _logger.LogInformation("Starting showtimes update.");
                    if (_showTimesProvider.GetAgeHours() < 4)
                    {
                        _logger.LogInformation("Showtimes age is less than 4 hours, not updating it.");
                        return;
                    }


                    var cinemas = _csfdDataProvider.GetAllShowTimes().ToList();
                    if (cinemas.Any())
                        _showTimesProvider.Set(cinemas);
                    else
                        throw new Exception("Failed to get any showtimes");
                    _logger.LogInformation("Showtimes update finished.");
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, e.Message);
                _sentryClient?.CaptureException(e);
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Showtimes update service is starting.");

            _timer = new Timer(Update, null, TimeSpan.Zero, TimeSpan.FromMinutes(60));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Showtimes update service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}