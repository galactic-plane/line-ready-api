using LineReadyApi.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace LineReadyApi
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                            .ConfigureWebHostDefaults(webBuilder =>
                            {
                                webBuilder.UseStartup<Startup>();
                            });
        }

        public static void InitializeDatabase(IHost host)
        {
            SeedDatabase(host);
        }

        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();
            InitializeDatabase(host);
            host.Run();
        }

        private static void SeedDatabase(IHost host)
        {
            using (IServiceScope scope = host.Services.CreateScope())
            {
                IServiceProvider services = scope.ServiceProvider;

                IWebHostEnvironment env = services.GetRequiredService<IWebHostEnvironment>();

                string cityDataJson = $"{Path.Combine(env.ContentRootPath, "wwwroot")}/data/us.city.list.json";
                string fishDataJson = $"{Path.Combine(env.ContentRootPath, "wwwroot")}/data/fisheries.noaa.gov.json";

                ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();

                try
                {
                    SeedData.InitializeAsync(services, cityDataJson, fishDataJson).Wait();
                    logger.LogInformation("The database is seeded.  Good job!");
                }
                catch (Exception ex)
                {                    
                    logger.LogError(ex, "An error occurred seeding the database.");
                }
            }
        }
    }
}