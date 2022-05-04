using System.Collections.Generic;
using System.Threading.Tasks;
using SampleCollector.Models;

namespace SampleCollector.Interfaces
{
    public interface ISampleCollectorRepository
    {
        Task<bool> AddCollectorCounterAsync(CollectorCounters counter);

        Task<bool> AddCollectorOptionsAsync(CollectorOptions options);

        Task<List<CollectorOptions>> GetCollectorOptionsAsync();

        Task<CollectorOptions> FindCollectorOptionsByNameAsync(string collectorName);

        Task<List<CollectorStatistics>> GetActiveStatisticsAsync();

        Task<bool> UpdateIsActive(CollectorOptionsUpdateModel model);

        Task<bool> SaveChangesAsync();
    }
}