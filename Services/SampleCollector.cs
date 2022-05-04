using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SampleCollector.Interfaces;
using SampleCollector.Models;

namespace SampleCollector.Services
{
    public class SampleCollector : ISampleCollector
    {
        private ConcurrentDictionary<string, long> _messageCounters = new ConcurrentDictionary<string, long>();

        private readonly IServiceProvider _serviceProvider;
        private readonly ICacheMonitor _cacheMonitor;

        public SampleCollector(IServiceProvider serviceProvider, ICacheMonitor cacheMonitor)
        {
            _serviceProvider = serviceProvider;
            _cacheMonitor = cacheMonitor;
        }

        /// <summary>
        /// Checks whether message needs to be sampled.
        /// </summary>
        /// <param name="key">An unique key that specifies the corresponding environment, for example "SOME_CLASS.SOME_METHOD.In"</param>
        /// <param name="binaryData">The content of the message"</param>
        public async Task ProcessMessageAsync(string key, BinaryData binaryData)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key));

            binaryData = binaryData == null || binaryData.ToString().Equals("")
                ? new BinaryData("{ \"error\":\"Message did not contain any data.\"}")
                : binaryData;
            
            CollectorOptions optionsForCollector = await _cacheMonitor.GetCollectorOptions(key);
            
            #region Null Checks & Limits Chck
            //check whether the collector exists within database & cache
            if (optionsForCollector == null || !optionsForCollector.IsActive)
                return;

            int totalSamplesAlltime = await _cacheMonitor.GetTotalSamplesAlltime(key);

            //check whether the collector is sampling within due date or if the collector reached max samples
            if (DateTimeOffset.UtcNow > optionsForCollector.ActiveUntil || totalSamplesAlltime >= optionsForCollector.MaxSamplesAlltime)
            {
                await _cacheMonitor.UpdateCollector(key, false);
                return;
            }

            //check whether the collector has reached the max amount of samples per day
            if (optionsForCollector.MaxSamplesDay != null)
            {
                int totalSamplesToday = await _cacheMonitor.GetTotalSamplesDay(key);
                if (totalSamplesToday >= optionsForCollector.MaxSamplesDay)
                    return;
            }

            //check whether the collector has collected the max amount of samples per hour
            if (optionsForCollector.MaxSamplesHour != null)
            {
                int totalSamplesHour = await _cacheMonitor.GetTotalSamplesHour(key);
                if (totalSamplesHour >= optionsForCollector.MaxSamplesHour)
                    return;
            }

            #endregion
            
            #region SampleCheck
            var currentIncomingMessageCount = _messageCounters.AddOrUpdate(key, 1, (_, oldValue) => oldValue + 1);

            int sampleEvery = (int)(100 / optionsForCollector.SamplePercentage);

            if (currentIncomingMessageCount % sampleEvery == 0)
            {
                SampleMessage message = new SampleMessage()
                {
                    Id = Guid.NewGuid(),
                    CollectorName = key,
                    MessageContent = binaryData
                };
                await SampleMessage(message);
            }
            #endregion
        }

        public async Task ProcessMessageAsync(BinaryData binaryData, CollectorOptions options)
        {
            var existsLocally = await _cacheMonitor.GetCollectorOptions(options.CollectorName) is not null;
            
            if(!existsLocally)
                await _cacheMonitor.WriteCollectorOptionsToCache(options);

            ProcessMessageAsync(options.CollectorName, binaryData);
        }
        public async Task ProcessMessageAsync(string key, string content)
        {
            await ProcessMessageAsync(key, new BinaryData(content));
        }
        private async Task SampleMessage(SampleMessage message)
        {
            #region UploadToAzure
            
            using var scope = _serviceProvider.CreateScope();
            var storageService = scope.ServiceProvider.GetService<IStorageService>();
            //bool uploadedSuccesfully = await storageService.UploadFromMemoryAsync(message.MessageContent.ToArray(), message.FileName);
            
            #endregion
            
            bool uploadedSuccesfully = true;

            #region Update MemoryCache
            if (uploadedSuccesfully)
                await _cacheMonitor.IncreaseCounter(message.CollectorName);
            #endregion
        }
    }
}