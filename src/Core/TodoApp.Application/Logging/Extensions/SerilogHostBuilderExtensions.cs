using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace TodoApp.Application.Logging.Extensions
{
    public static class SerilogHostBuilderExtensions
    {
        public static IHostBuilder UseSerilogLogging(this IHostBuilder hostBuilder, bool clearProviders = false, string sectionName = "SeriLog")
        {
            if (clearProviders)
            {
                hostBuilder.ConfigureLogging(logConfig => logConfig.ClearProviders());
            }

            return hostBuilder.UseSerilog((hostContext, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration, sectionName);
            });
        }
    }
}
