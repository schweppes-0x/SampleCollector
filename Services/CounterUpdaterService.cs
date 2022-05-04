using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using SampleCollector.Interfaces;

namespace SampleCollector.Services
{
    public class CounterUpdaterService : IHostedService, IDisposable
    {
        private readonly ICacheMonitor _cacheMonitor;
        private Timer _timer;

        public CounterUpdaterService(ICacheMonitor cacheMonitor)
        {
            _cacheMonitor = cacheMonitor;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cacheMonitor.UpdateCounterCache();
            TimeSpan spanToNextHour = (DateTime.UtcNow.Date.AddHours(DateTime.UtcNow.Hour + 1) - DateTime.UtcNow) + TimeSpan.FromMinutes(1);
            _timer = new Timer(UpdateCounters, null, spanToNextHour, TimeSpan.FromHours(1));
        }

        private void UpdateCounters(object state)
        {
            try
            {
                _cacheMonitor.UpdateCounterCache().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                // ignored
                Console.WriteLine($"[!] - Error while updating counters in cache : {e}");
            }
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