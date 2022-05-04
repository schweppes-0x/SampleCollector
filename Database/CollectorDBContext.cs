using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SampleCollector.Models;

namespace SampleCollector.Database
{
    public class CollectorDBContext : DbContext
    {
        public CollectorDBContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<CollectorOptions> CollectorOptions { get; set; }
        public virtual DbSet<CollectorCounters> CollectorCounters { get; set; }

        public DbSet<CollectorStatistics> ActiveCollectorStatistics { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            builder.Entity<CollectorCounters>().HasKey(e => new { e.CollectorName, e.TimeStamp });

            builder.Entity<CollectorStatistics>(e =>
            {
                e.HasNoKey()
                .ToSqlQuery(@"
                                   SELECT
                                        options.CollectorName,
                                        AllTime       = ISNULL(SUM(count), 0),
                                        Today         = ISNULL(SUM(CASE WHEN TimeStamp >= CAST(CAST(GETUTCDATE() AS date) AS datetime)
                                                                        AND TimeStamp < CAST(DATEADD(day, 1, CAST(GETUTCDATE() AS date)) AS datetime)
                                                                        THEN count END), 0)
                                        FROM CollectorOptions options
                                        JOIN CollectorCounters counters ON counters.CollectorName = options.CollectorName
                                        WHERE options.IsActive = 1
                                        GROUP BY
                                            options.CollectorName;
                                       ");
            });
        }
    }
}