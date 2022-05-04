using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Swashbuckle.AspNetCore.Filters;

#nullable disable

namespace SampleCollector.Models
{
    public partial class CollectorCounters
    {
        public CollectorCounters()
        {
        }

        public CollectorCounters(string collectorName, DateTimeOffset timeStamp, int count)
        {
            CollectorName = collectorName;
            TimeStamp = timeStamp;
            Count = count;
        }

        /// <example>example_name</example>
        [Key, Column("CollectorName", TypeName = "varchar(350)")]
        public string CollectorName { get; set; }

        /// <example>2021-12-15T14:57:38.8859778+00:00/</example>
        [Key, Column("TimeStamp", TypeName = "datetimeoffset(7)")]
        public DateTimeOffset TimeStamp { get; set; }

        /// <example>18</example>
        [Column("Count", TypeName = "int")]
        public int Count { get; set; }
    }

    public class CollectorCountersExample : IExamplesProvider<CollectorCounters>
    {
        private static CollectorCounters _example = new CollectorCounters { CollectorName = "collectorX", TimeStamp = DateTime.UtcNow, Count = 16 }; public static CollectorCounters Example
        {
            get => _example;
        }

        public static CollectorCounters Example1 { get => _example; }

        public CollectorCounters GetExamples() => _example;
    }
}