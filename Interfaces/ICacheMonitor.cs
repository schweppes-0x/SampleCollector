using System.Threading.Tasks;
using SampleCollector.Models;

namespace SampleCollector.Interfaces
{
    public interface ICacheMonitor
    {
        Task UpdateOptionsCache();

        Task UpdateCounterCache();

        Task<bool> UpdateCollector(string collectorName, bool isActive);

        Task<bool> UpdateCollector(CollectorOptionsUpdateModel model);

        Task<CollectorOptions> GetCollectorOptions(string collectorName);

        Task<int> GetTotalSamplesHour(string collectorName);

        Task<int> GetTotalSamplesDay(string collectorName);

        Task<int> GetTotalSamplesAlltime(string collectorName);

        Task IncreaseCounter(string collectorname);

        Task WriteCollectorOptionsToCache(CollectorOptions options);

    }
}