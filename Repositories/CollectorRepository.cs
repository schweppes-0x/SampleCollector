using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SampleCollector.Database;
using SampleCollector.Interfaces;
using SampleCollector.Models;

namespace SampleCollector.Repositories
{
    public class SampleCollectorRepository : ISampleCollectorRepository
    {
        private readonly CollectorDBContext _context;

        public SampleCollectorRepository(CollectorDBContext context)
        {
            _context = context;
        }

        public async Task<bool> AddCollectorOptionsAsync(CollectorOptions options)
        {
            if (options == null)
                return false;

            await _context.CollectorOptions.AddAsync(options);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddCollectorCounterAsync(CollectorCounters counter)
        {
            await _context.CollectorCounters.AddAsync(counter);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<CollectorOptions>> GetCollectorOptionsAsync()
        {
            var list = await _context.CollectorOptions.ToListAsync();
            return list;
        }

        public async Task<List<CollectorStatistics>> GetActiveStatisticsAsync()
        {
            var list = await _context.ActiveCollectorStatistics.ToListAsync();
            return list;
        }

        public async Task<CollectorOptions> FindCollectorOptionsByNameAsync(string collectorName)
        {
            var options = await _context.CollectorOptions.Where(options => options.CollectorName == collectorName).FirstOrDefaultAsync();
            return options;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateIsActive(CollectorOptionsUpdateModel model)
        {
            var options = await _context.CollectorOptions.Where(collectorOptions => collectorOptions.CollectorName == model.CollectorName).FirstOrDefaultAsync();

            options.IsActive = model.IsActive;

            return await SaveChangesAsync();
        }
    }
}