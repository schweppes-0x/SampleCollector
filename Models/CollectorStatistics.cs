using System.ComponentModel.DataAnnotations.Schema;
using Swashbuckle.AspNetCore.Filters;

namespace SampleCollector.Models
{
    public class CollectorStatistics
    {
        [Column("CollectorName", TypeName = "varchar(350)")]
        public string CollectorName { get; set; }

        [Column("AllTime", TypeName = "int")]
        public int AllTime { get; set; }

        [Column("Today", TypeName = "int")]
        public int Today { get; set; }
    }

    public class CollectorStatisticsExample : IExamplesProvider<CollectorStatistics>
    {
        private static CollectorStatistics _example = new CollectorStatistics
        {
            AllTime = 350,
            CollectorName = "collectorX",
            Today = 5
        };

        public static CollectorStatistics Example { get => _example; }

        public CollectorStatistics GetExamples() => _example;
    }
}