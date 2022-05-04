using Microsoft.Extensions.DependencyInjection;
using SampleCollector.Interfaces;
using SampleCollector.Repositories;
using SampleCollector.Services;

namespace SampleCollector.Extensions
{
    internal static class SampleCollectorExtensions
    {
        public static void ConfigureSampleCollectorServices(this IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddHostedService<CacheUpdaterService>();
            services.AddHostedService<CounterUpdaterService>();
            services.AddHostedService<MessagesReceiverService>();

            
            services.AddSingleton<ISampleCollector, Services.SampleCollector>();

            services.AddScoped<IStorageService, StorageService>();
            services.AddSingleton<ICacheMonitor, CacheMonitor>();
            services.AddScoped<ISampleCollectorRepository, SampleCollectorRepository>();

            services.ConfigureCustomSwaggerExamples();
        }
    }
}