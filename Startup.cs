using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SampleCollector.Extensions;
using SampleCollector.Database;
using SampleCollector.Options;

namespace SampleCollector
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AzureStorageOptions>(Configuration.GetSection("SampleCollector:AzureStorageBlob"));
            services.Configure<CacheMonitorOptions>(Configuration.GetSection("SampleCollector:CacheUpdater"));

            services.ConfigureSampleCollectorServices();

            services.AddControllers();

            services.AddDbContext<CollectorDBContext>(options =>
            {
                options.UseSqlServer(Configuration["SampleCollector:Database:ConnectionString"]);
            });
        }
        public void Configure(IApplicationBuilder builder, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                builder.UseDeveloperExceptionPage();
                builder.UseSwagger();
                builder.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1.2"));
            }

            builder.UseHttpsRedirection();

            builder.UseRouting();

            builder.UseAuthorization();

            builder.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}