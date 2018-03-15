using System;
using System.Threading.Tasks;
using KinoDnesApi.Model;
using Microsoft.Extensions.Options;
using SharpRaven;
using SharpRaven.Data;

namespace KinoDnesApi
{
    public class SentryClient : ISentryClient
    {
        readonly RavenClient _ravenClient;

        public SentryClient(IOptions<EnvironmentVariables> configuration)
        {
            var dsn = configuration.Value.SentryDsn;
            _ravenClient = new RavenClient(dsn);
        }

        public async Task CaptureException(Exception ex)
        {
            await _ravenClient.CaptureAsync(new SentryEvent(ex));
        }
    }
}