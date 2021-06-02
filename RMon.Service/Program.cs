using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RMon.Configuration.MassTransit;
using System.IO;

namespace RMon.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var currentDir = System.AppContext.BaseDirectory;
            Directory.SetCurrentDirectory(currentDir);

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .SetMassTransitConfiguration(args)
                .ConfigureLogging((hostContext, builder) => { builder.AddLog4Net(); })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                })
                .UseSystemd()
                .UseWindowsService();
    }
}
