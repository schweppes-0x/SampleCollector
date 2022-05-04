using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SampleCollector.Interfaces;
using SampleCollector.Options;

namespace SampleCollector.Services
{
    public class CacheUpdaterService : BackgroundService
    {
        private readonly IOptions<CacheMonitorOptions> _cacheMonitorOptions;
        private readonly ICacheMonitor _cacheMonitor;

        public CacheUpdaterService(IOptions<CacheMonitorOptions> cacheMonitorOptions, ICacheMonitor cacheMonitor)
        {
            _cacheMonitorOptions = cacheMonitorOptions;
            _cacheMonitor = cacheMonitor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _cacheMonitor.UpdateOptionsCache();
                await Task.Delay(_cacheMonitorOptions.Value.Interval);
            }
        }
    }
}