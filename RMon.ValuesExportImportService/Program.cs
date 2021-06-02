using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RMon.Configuration.DependencyInjection;
using RMon.Configuration.MassTransit;
using RMon.Configuration.Options;

namespace RMon.ValuesExportImportService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .SetMassTransitConfiguration(args)
                .ConfigureLogging((context, builder) => { builder.AddLog4Net(); })
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<Service>(hostContext.Configuration.GetSection(nameof(Service)));
                    services.ConfigureOption<Esb>(hostContext.Configuration, nameof(ValuesExportImportService), nameof(Esb));
                    services.ConfigureOption<EntitiesDatabase>(hostContext.Configuration, nameof(ValuesExportImportService), nameof(EntitiesDatabase));

                    services.AddHostedService<Worker>();
                })
                .UseSystemd()
                .UseWindowsService();
    }
}
