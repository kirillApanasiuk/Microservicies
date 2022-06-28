using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app,bool isProd)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var dbcontext = serviceScope.ServiceProvider.GetService<AppDbContext>();

                if (dbcontext == null)
                {
                    throw new Exception($"Can not get instance from container {typeof(AppDbContext)}");
                }

                SeedData(dbcontext,isProd);
            }
        }

        private static void SeedData(AppDbContext context,bool isProd)
        {
            if (isProd)
            {
                Console.WriteLine("--> Attempting to apply migrations");
                try
                {

                context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not run any migrations: {ex}");
                    throw;
                }
            }
            if (!context.Platforms.Any())
            {
                Console.WriteLine("--> Seeding Data");
                context.Platforms.AddRange(
                    new Platform { Name = "Dot net", Publisher = "Microsoft", Cost = "Free" },
                    new Platform { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
                    new Platform { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" });

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> We already have data");
            }
        }
    }
}
