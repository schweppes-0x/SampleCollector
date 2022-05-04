using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Swashbuckle.AspNetCore.Filters;

#nullable disable

namespace SampleCollector.Models
{
    [Table("CollectorOptions")]
    public class CollectorOptions
    {
        [Key, Required, Column("CollectorName", TypeName = "varchar(350)")]
        public string CollectorName { get; set; }

        [Required, Column("IsActive", TypeName = "bit")]
        public bool IsActive { get; set; }

        [Column("MaxSamplesHour", TypeName = "int")]
        public int? MaxSamplesHour { get; set; }

        [Column("MaxSamplesDay", TypeName = "int")]
        public int? MaxSamplesDay { get; set; }

        [Required, Column("MaxSamplesAlltime", TypeName = "int")]
        public int MaxSamplesAlltime { get; set; }

        [Required, Column("ActiveUntil", TypeName = "datetimeoffset(7)")]
        public DateTimeOffset ActiveUntil { get; set; }

        [Required, Column("SamplePercentage", TypeName = "decimal(7,4)")]
        public decimal SamplePercentage { get; set; }

        [Column("CanNotify", TypeName = "bit")]
        public bool? CanNotify { get; set; }

        public CollectorOptionsUpdateModel ToUpdateModel()
        {
            return new CollectorOptionsUpdateModel(CollectorName, IsActive);
        }
    }

    public class CollectorOptionsUpdateModel
    {
        public CollectorOptionsUpdateModel(string collectorName, bool isActive)
        {
            CollectorName = collectorName;
            IsActive = isActive;
        }
        public string CollectorName { get; set; }
        public bool IsActive { get; set; }
    }

    public class CollectorOptionsExample : IExamplesProvider<CollectorOptions>
    {
        private static CollectorOptions _example = new CollectorOptions
        {
            CollectorName = "collectorX",
            ActiveUntil = DateTimeOffset.UtcNow,
            CanNotify = false,
            IsActive = true,
            MaxSamplesAlltime = 300,
            MaxSamplesDay = 15,
            MaxSamplesHour = 3,
            SamplePercentage = new decimal(0.15)
        };

        public static CollectorOptions Example { get => _example; }

        public CollectorOptions GetExamples() => _example;
    }
}