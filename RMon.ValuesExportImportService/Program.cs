using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using Microsoft.Extensions.Logging;
using RMon.Configuration.DependencyInjection;
using RMon.Configuration.MassTransit;
using RMon.Configuration.Options;
using RMon.Configuration.Options.FileStorage;
using RMon.Context.FrontEndContext;
using RMon.Core.Base;
using RMon.ValuesExportImportService.Data;
using RMon.ValuesExportImportService.Excel;
using RMon.ValuesExportImportService.Files;
using RMon.ValuesExportImportService.Globalization;
using RMon.ValuesExportImportService.Processing.Export;
using RMon.ValuesExportImportService.Processing.Import;
using RMon.ValuesExportImportService.Processing.Parse;
using RMon.ValuesExportImportService.ServiceBus;
using RMon.ValuesExportImportService.ServiceBus.Export;
using RMon.ValuesExportImportService.ServiceBus.Import;
using RMon.ValuesExportImportService.ServiceBus.Parse;

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
                .ConfigureLogging((context, builder) =>
                {
                    builder.SetMinimumLevel(LogLevel.Trace);
                    builder.AddLog4Net();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<Service>(hostContext.Configuration.GetSection(nameof(Service)));
                    services.ConfigureOption<TasksEsb>(hostContext.Configuration, nameof(ValuesExportImportService), nameof(TasksEsb));
                    services.ConfigureOption<EntitiesDatabase>(hostContext.Configuration, nameof(ValuesExportImportService), nameof(EntitiesDatabase));
                    services.ConfigureOption<ValuesExportImportFileStorage>(hostContext.Configuration, nameof(ValuesExportImportService), nameof(ValuesExportImportFileStorage));
                    
                    services.AddSingleton<IBusProvider, BusProvider>();
                    services.AddSingleton<ExportStateMachine>();
                    services.AddSingleton<ParseStateMachine>();
                    services.AddSingleton<ImportStateMachine>();
                    services.AddSingleton<IExportBusPublisher, ExportBusPublisher>();
                    services.AddSingleton<IParseBusPublisher, ParseBusPublisher>();
                    services.AddSingleton<IImportBusPublisher, ImportBusPublisher>();

                    services.AddSingleton<ImportTaskLogger>();
                    services.AddSingleton<ExportTaskLogger>();
                    services.AddSingleton<IExportTaskLogic, ExportTaskLogic>();
                    services.AddSingleton<IImportTaskLogic, ImportTaskLogic>();
                    services.AddSingleton<IParseTaskLogic, ParseTaskLogic>();

                    services.AddSingleton<IRepositoryFactoryConfigurator, RepositoryFactoryConfigurator>();
                    services.AddSingleton<ISimpleFactory<FrontEndContext>, FrontEndContextFactory>();
                    services.AddSingleton<IDataRepository, MsSqlDataRepository>();
                   

                    services.AddSingleton<IFileStorage, Files.FileStorage>();
                    services.AddSingleton<IExcelWorker, ExcelWorker>();


                    services.AddSingleton<ILanguageRepository, LanguageRepository>();
                    services.AddSingleton<IGlobalizationProviderFactory, FileProviderFactory>();

                    services.AddHostedService<BusService>();
                })
                .UseSystemd()
                .UseWindowsService();
    }
}
