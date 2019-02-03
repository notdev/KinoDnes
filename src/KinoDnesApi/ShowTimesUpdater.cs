using System;
using System.Linq;
using KinoDnesApi.DataProviders;
using Microsoft.Extensions.Logging;
using Sentry;

namespace KinoDnesApi
{
    public class ShowTimesUpdater
    {
        private readonly IFileSystemShowTimes _fileSystemShowTimes;
        private readonly ICsfdDataProvider _csfdDataProvider;
        private readonly ILogger<ShowTimesUpdater> _logger;
        private readonly ISentryClient _sentryClient;
        private static readonly object _lock = new object();

        public ShowTimesUpdater(IFileSystemShowTimes fileSystemShowTimes, ICsfdDataProvider csfdDataProvider,
            ILogger<ShowTimesUpdater> logger, IServiceProvider serviceProvider)
        {
            _fileSystemShowTimes = fileSystemShowTimes;
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

        public void Update()
        {
            try
            {
                lock (_lock)
                {
                    _logger.LogInformation("Starting showtimes update.");
                    var showTimes = _csfdDataProvider.GetAllShowTimes().ToList();
                    if (showTimes.Any())
                        _fileSystemShowTimes.Set(showTimes);
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
    }
}