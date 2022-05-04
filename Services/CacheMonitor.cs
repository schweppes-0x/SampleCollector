using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SampleCollector.Interfaces;
using SampleCollector.Models;

namespace SampleCollector.Services
{
    internal class CacheMonitor : ICacheMonitor
    {
        private readonly IMemoryCache _cache;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CacheMonitor> _logger;

        public CacheMonitor(
            IMemoryCache cache,
            IServiceProvider repository,
            ILogger<CacheMonitor> logger)
        {
            _cache = cache;
            _serviceProvider = repository;
            _logger = logger;
        }

        public Task<CollectorOptions> GetCollectorOptions(string collectorName)
        {
            return Task.FromResult((CollectorOptions)_cache.Get(collectorName));
        }

        public async Task UpdateOptionsCache()
        {
            _logger.LogInformation("[!] - updating cache..");

            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetService<ISampleCollectorRepository>();
            
            if (repository != null)
            {
                var collectorOptionsList = await repository.GetCollectorOptionsAsync();

                foreach (var options in collectorOptionsList)
                    await WriteCollectorOptionsToCache(options);
            
                _logger.LogInformation("[!] - updated cache");
            }
        }

        public async Task UpdateCounterCache()
        {
            _logger.LogInformation("[!] - updating counters in cache..");

            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetService<ISampleCollectorRepository>();

            if (repository != null)
            {
                var activeList = await repository.GetActiveStatisticsAsync();

                foreach (var item in activeList)
                    await WriteCounterToCache(item);
                
                _logger.LogInformation("[!] - updated counters in cache..");
            }
            else
            {
                _logger.LogInformation("[!] Error while updating counters in cache. [Repository was NULL]");
            }
        }

        private Task WriteCounterToCache(CollectorStatistics collectorStats)
        {
            var countAlltime = collectorStats.AllTime;
            var countToday = collectorStats.Today;

            #region NULL checks
            
            countAlltime = countAlltime == null ? 0 : countAlltime;
            countToday = countToday == null ? 0 : countToday;
            
            #endregion
            
            #region Set Keys
            
            var keyForDay = $"{collectorStats.CollectorName}::";
            var keyForAlltime = $"{collectorStats.CollectorName}:";

            #endregion
            
            _cache.Set<int>(keyForDay, countToday);
            _cache.Set<int>(keyForAlltime, countAlltime);
            return Task.CompletedTask;
        }

        public async Task<bool> UpdateCollector(string collectorName, bool isActive)
        {
            var updateModel = new CollectorOptionsUpdateModel(collectorName, isActive);
            return await UpdateCollector(updateModel);
        }

        public async Task<bool> UpdateCollector(CollectorOptionsUpdateModel model)
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetService<ISampleCollectorRepository>();

            //update database first
            var changed = repository != null && await repository.UpdateIsActive(model);

            if (changed) //if database has been changed, update the cache value
            {
                GetCollectorOptions(model.CollectorName).Result.IsActive = false;
                _logger.LogInformation($"[!] - Collector {model.CollectorName} has been succesfully changed.");
            }
            else
                _logger.LogInformation($"[!] - Collector {model.CollectorName} could not be changed.");

            return changed;
        }

        public Task<int> GetTotalSamplesHour(string collectorName)
        {
            var key = $"{collectorName}:::{DateTimeOffset.UtcNow.Hour}";

            var counter = _cache.Get<CacheCounterItem>(key);
            return Task.FromResult(counter?.Count ?? 0);
        }

        public async Task<int> GetTotalSamplesDay(string collectorName)
        {
            var key = $"{collectorName}::";
            var currentHourCount = await GetTotalSamplesHour(collectorName);
            return _cache.Get<int>(key) + currentHourCount;
        }

        public async Task<int> GetTotalSamplesAlltime(string collectorName)
        {
            var key = collectorName + ":";
            var currentHourCount = await GetTotalSamplesHour(collectorName);

            return _cache.Get<int>(key) + currentHourCount;
        }

        public Task WriteCollectorOptionsToCache(CollectorOptions options)
        {
            _cache.Set<CollectorOptions>(options.CollectorName, options);
            return Task.CompletedTask;
        }

        public Task IncreaseCounter(string collectorName)
        {
            var key = $"{collectorName}:::{DateTimeOffset.UtcNow.Hour}";
            var cachedItem = _cache.Get<CacheCounterItem>(key);

            if (cachedItem is null)
            {
                var spanToNextHour = DateTime.UtcNow.Date.AddHours(DateTime.UtcNow.Hour + 1) - DateTime.UtcNow;

                var options = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTimeOffset.UtcNow + spanToNextHour
                }
                .RegisterPostEvictionCallback(UpdateDatabase);

                var createdCacheItem = new CacheCounterItem { CollectorName = collectorName, Count = 1, Options = options };

                _cache.Set(key, createdCacheItem, options);
            }
            else
            {
                cachedItem.Count++;
                _cache.Set(key, cachedItem, cachedItem.Options);
            }

            return Task.CompletedTask;
        }

        private async void UpdateDatabase(object key, object value, EvictionReason reason, object state)
        {
            if (reason is not EvictionReason.Expired)
                return;

            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetService<ISampleCollectorRepository>();

            var collectorName = ((CacheCounterItem)value).CollectorName;
            var currentCount = ((CacheCounterItem)value).Count;
            var timeStamp = ((CacheCounterItem)value).Options.AbsoluteExpiration.Value.AddHours(-1);

            var counter = new CollectorCounters(collectorName, timeStamp, currentCount);

            var updated = repository != null && await repository.AddCollectorCounterAsync(counter);

            _logger.LogInformation($"Key: {key} in cache is expired, database updated: {updated}");
        }
    }

    internal class CacheCounterItem
    {
        public string CollectorName { get; set; }
        public int Count { get; set; }
        public MemoryCacheEntryOptions Options { get; set; }
    }
}