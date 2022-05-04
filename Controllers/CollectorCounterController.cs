using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SampleCollector.Interfaces;
using SampleCollector.Models;

namespace SampleCollector.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CollectorCounterController : ControllerBase
    {
        private readonly ICacheMonitor _cacheMonitor;
        private readonly ISampleCollectorRepository collectorRepository;

        public CollectorCounterController(ICacheMonitor cacheMonitor, ISampleCollectorRepository collectorRepository)
        {
            _cacheMonitor = cacheMonitor;
            this.collectorRepository = collectorRepository;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="collectorName" example="collectorX">The name of the collector we're searching details for.</param>
        /// <returns>If there was no such object "0" will be returned otherwise it will get the current total count of samples in the last hour.</returns>
        [HttpGet("count/{collectorName}")]
        public long Get(string collectorName)
        {
            return _cacheMonitor.GetTotalSamplesHour(collectorName).Result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>Retrieves all the found statistics</returns>
        [HttpGet("statistics")]
        public async Task<ActionResult<List<CollectorStatistics>>> GetAllActive()
        {
            var list = await collectorRepository.GetActiveStatisticsAsync();
            return Ok(list);
        }

        [HttpPut("counter")]
        public async Task<ActionResult<bool>> AddCollectorCounter([FromBody] CollectorCounters counter)
        {
            var added = await collectorRepository.AddCollectorCounterAsync(counter);
            return Ok(added);
        }
    }
}