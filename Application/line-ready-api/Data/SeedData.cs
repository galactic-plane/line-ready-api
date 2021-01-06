using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LineReadyApi.Data
{

    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider service, string cityDataJson, string fishDataJson)
        {
            await AddTestData(service.GetRequiredService<LineReadyApiDbContext>(), cityDataJson, fishDataJson);
        }

        public static async Task AddTestData(LineReadyApiDbContext context, string cityDataJson, string fishDataJson)
        {
            if (context.Cities.Any())
            {
                // Already has data
                return;
            }

            using (StreamReader r = new StreamReader(cityDataJson))
            {
                IEnumerable<CityEntity> source;
                string json = r.ReadToEnd();
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    IgnoreNullValues = true,
                    WriteIndented = true
                };
                source = JsonSerializer.Deserialize<RootEntity>(json, options).CityData;
                context.AddRange(source);
                await context.SaveChangesAsync();
            }

            if (context.Fish.Any())
            {
                // Already has data
                return;
            }

            using (StreamReader r = new StreamReader(fishDataJson))
            {
                IEnumerable<FishEntity> source;
                string json = r.ReadToEnd();
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    IgnoreNullValues = true,
                    WriteIndented = true
                };
                source = JsonSerializer.Deserialize<RootEntity>(json, options).FishData;
                context.AddRange(source);
                await context.SaveChangesAsync();
            }
        }
    }

}