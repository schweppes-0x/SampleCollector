using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SampleCollector.Interfaces;
using SampleCollector.Models;

namespace SampleCollector.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CollectorOptionsController : ControllerBase
    {
        private readonly ICacheMonitor _cacheMonitor;
        public CollectorOptionsController(ICacheMonitor cacheMonitor)
        {
            _cacheMonitor = cacheMonitor;
        }

        /// <summary>
        /// Searches for collector which has the same collectorName as the one provided.
        /// </summary>
        /// <param name="collectorName"></param>
        /// <returns></returns>
        [HttpGet("options/{collectorName}")]
        public async Task<ActionResult<CollectorOptions>> FindCollectorOptions(string collectorName)
        {
            return await _cacheMonitor.GetCollectorOptions(collectorName);
        }

        [HttpPost("options/{collectorName}/{isActive}")]
        public async Task<ActionResult<bool>> Change(string collectorName, bool isActive)
        {
            return await _cacheMonitor.UpdateCollector(collectorName, isActive);
        }
    }
}