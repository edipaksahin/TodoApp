using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace TodoApp.Persistence.Extensions
{
    public static class DbContextHelpers
    {
        public static void InitializeDatabase<TDbContext>(
            this IApplicationBuilder app,
            bool recreateOnStartup = false,
            bool shouldSeed = false,
            Func<IServiceProvider, TDbContext, Task> seed = null,
            bool recreateOnFailure = false) where TDbContext : DbContext
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            var services = serviceScope.ServiceProvider;

            var databaseContext = services.GetRequiredService<TDbContext>();

            if (recreateOnStartup)
            {
                databaseContext.Database.EnsureDeleted();
            }

            try
            {
                databaseContext.Database.Migrate();
            }
            catch (Exception)
            {
                if (recreateOnFailure)
                {
                    app.InitializeDatabase(true, shouldSeed, seed, false);
                }
                else
                {
                    throw;
                }
            }

            if (shouldSeed && seed != null)
            {
                seed?.Invoke(services, databaseContext).GetAwaiter().GetResult();
            }
        }
    }
}
