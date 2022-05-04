using System;

namespace SampleCollector.Options
{
    public class CacheMonitorOptions
    {
        public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(10);
    }
}