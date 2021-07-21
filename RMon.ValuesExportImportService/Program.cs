using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using RMon.Configuration.DependencyInjection;
using RMon.Configuration.MassTransit;
using RMon.Configuration.Options;
using RMon.Configuration.Options.FileStorage;
using RMon.Configuration.Options.TagValueTransformation;
using RMon.Context.BackEndContext;
using RMon.Context.FrontEndContext;
using RMon.Core.Base;
using RMon.Data.Provider.Units.Backend.Interfaces;
using RMon.Data.Provider.Units.Backend.Sql;
using RMon.Data.Provider.Values;
using RMon.ValuesExportImportService.Configuration;
using RMon.ValuesExportImportService.Data;
using RMon.ValuesExportImportService.Excel.Flexible;
using RMon.ValuesExportImportService.Excel.Matrix;
using RMon.ValuesExportImportService.Excel.Table;
using RMon.ValuesExportImportService.Files;
using RMon.ValuesExportImportService.Globalization;
using RMon.ValuesExportImportService.Processing.Common;
using RMon.ValuesExportImportService.Processing.Export;
using RMon.ValuesExportImportService.Processing.Import;
using RMon.ValuesExportImportService.Processing.Parse;
using RMon.ValuesExportImportService.Processing.Parse.Format80020;
using RMon.ValuesExportImportService.Processing.Permission;
using RMon.ValuesExportImportService.ServiceBus;
using RMon.ValuesExportImportService.ServiceBus.Export;
using RMon.ValuesExportImportService.ServiceBus.Import;
using RMon.ValuesExportImportService.ServiceBus.Parse;

[assembly: InternalsVisibleTo("RMon.ValuesExportImportService.Debug")]
[assembly: InternalsVisibleTo("RMon.ValuesExportImportService.Tests")]
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
                    services.ConfigureOption<ValuesDatabase>(hostContext.Configuration, nameof(ValuesExportImportService), nameof(ValuesDatabase));
                    services.ConfigureOption<ValuesExportImportFileStorage>(hostContext.Configuration, nameof(ValuesExportImportService), nameof(ValuesExportImportFileStorage));
                    services.ConfigureOption<ValuesParseOptions>(hostContext.Configuration, nameof(ValuesExportImportService), nameof(ValuesParseOptions));
                    services.ConfigureOption<TagValueTransformation>(hostContext.Configuration, nameof(ValuesExportImportService), nameof(TagValueTransformation));
                    services.ConfigureOption<ResultMessageSenderOptions>(hostContext.Configuration, nameof(ValuesExportImportService), nameof(ResultMessageSenderOptions));
                    services.ConfigureOption<ValuesLoggingOptions>(hostContext.Configuration, nameof(ValuesExportImportService), nameof(ValuesLoggingOptions));


                    services.AddSingleton<IBusProvider, BusProvider>();
                    services.AddSingleton<ExportStateMachine>();
                    services.AddSingleton<ParseStateMachine>();
                    services.AddSingleton<ImportStateMachine>();
                    services.AddSingleton<IExportBusPublisher, ExportBusPublisher>();
                    services.AddSingleton<IParseBusPublisher, ParseBusPublisher>();
                    services.AddSingleton<IImportBusPublisher, ImportBusPublisher>();

                    services.AddSingleton<IRepositoryFactoryConfigurator, RepositoryFactoryConfigurator>();
                    services.AddSingleton<ISimpleFactory<BackEndContext>, BackEndContextFactory>();
                    services.AddSingleton<ISimpleFactory<FrontEndContext>, FrontEndContextFactory>();
                    services.AddSingleton<ISimpleFactory<IValueRepository>, ValueRepositorySimpleFactory>();
                    services.AddSingleton<IDataRepository, MsSqlDataRepository>();
                    services.AddSingleton<ILogicDevicesRepository, SqlLogicDevicesRepository>();
                    services.AddSingleton<ITagsRepository, SqlTagsRepository>();

                    services.AddSingleton<IEntityReader, EntityReader>();
                    services.AddSingleton<IFileStorage, Files.FileStorage>();
                    services.AddSingleton<Format80020Parser>();
                    services.AddSingleton<IExcelWorker, ExcelWorker>();
                    services.AddSingleton<Matrix24X31Reader>();
                    services.AddSingleton<Matrix31X24Reader>();
                    services.AddSingleton<ITableReader, TableReader>();

                    services.AddSingleton<DbValuesAnalyzer>();
                    services.AddSingleton<ParseXml80020Logic>();
                    services.AddSingleton<ParseMatrix24X31Logic>();
                    services.AddSingleton<ParseMatrix31X24Logic>();
                    services.AddSingleton<ParseTableLogic>();
                    services.AddSingleton<ParseFlexibleFormatLogic>();

                    services.AddSingleton<ITransformationRatioCalculator, TransformationRatioCalculator>();
                    services.AddSingleton<IResultMessagesSender, ResultMessagesSqlProvider>();
                    services.AddSingleton<IValuesLogger, ValuesLogger>();

                    services.AddSingleton<IImportTaskLogger, ImportTaskLogger>();
                    services.AddSingleton<IParseTaskLogger, ParseTaskLogger>();
                    services.AddSingleton<IExportTaskLogger, ExportTaskLogger>();
                    services.AddSingleton<IPermissionLogic, PermissionLogic>();
                    services.AddSingleton<IExportTaskLogic, ExportTaskLogic>();
                    services.AddSingleton<IParseTaskLogic, ParseTaskLogic>();
                    services.AddSingleton<IImportTaskLogic, ImportTaskLogic>();


                    services.AddSingleton<ILanguageRepository, LanguageRepository>();
                    services.AddSingleton<IGlobalizationProviderFactory, FileProviderFactory>();

                    services.AddHostedService<BusService>();
                })
                .UseSystemd()
                .UseWindowsService();
    }
}
