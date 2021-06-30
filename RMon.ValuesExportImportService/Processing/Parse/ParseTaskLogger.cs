using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RMon.Configuration.Options;
using RMon.Core.CommonTask;
using RMon.Data.Provider.Esb.Entities;
using RMon.Data.Provider.Esb.Entities.ValuesExportImport;
using RMon.ESB.Core.Common;
using RMon.Globalization.String;
using RMon.Values.ExportImport.Core;
using RMon.ValuesExportImportService.Data;
using RMon.ValuesExportImportService.Processing.Common;
using RMon.ValuesExportImportService.ServiceBus.Parse;

namespace RMon.ValuesExportImportService.Processing.Parse
{
    class ParseTaskLogger : BaseTaskLogger<DbValuesExportImportTask>, IParseTaskLogger
    {
        /// <summary>
        /// Конструктор 1
        /// </summary>
        /// <param name="serviceOptions">Опции сервиса</param>
        /// <param name="repositoryFactoryConfigurator">Конфигуратор репозиториев</param>
        /// <param name="dataRepository">Репозиторий данных</param>
        /// <param name="fileLogger">Логгер для записи логов в файл</param>
        /// <param name="esbLogger">Логгер для передачи логов по ESB</param>
        public ParseTaskLogger(IOptionsMonitor<Service> serviceOptions, IRepositoryFactoryConfigurator repositoryFactoryConfigurator, IDataRepository dataRepository, ILogger<ParseTaskLogger> fileLogger, IParseBusPublisher esbLogger)
            : base(serviceOptions, repositoryFactoryConfigurator, dataRepository, fileLogger, esbLogger)
        {
        }

        
        /// <inheritdoc/>
        public async Task LogFinishedAsync(ITask receivedTask, DbValuesExportImportTask dbTask, I18nString msg, IList<ValueInfo> values)
        {
            dbTask.ParseResults = new ValuesParseTaskResults
            {
                Values = values
            };

            FileLogger.LogInformation($"{receivedTask.CorrelationId}: {msg}");
            var currentDate = await DataRepository.GetDateAsync().ConfigureAwait(false);

            dbTask.State = TaskState.Success;
            dbTask.Progress = 100;
            dbTask.FinishDate = currentDate;
            dbTask.UpdateDate = currentDate;
            await DbLogger.UpdateTaskAsync(dbTask).ConfigureAwait(false);

            await DbLogger.AddToTaskLogAsync<DbValuesExportImportTask>(dbTask.Id, currentDate, DbTaskLogLevel.Info, msg.ToJsonString()).ConfigureAwait(false);

            await EsbLogger.SendTaskLogAsync(receivedTask, currentDate, msg.ToString()).ConfigureAwait(false);
            await EsbLogger.SendTaskProgressionChangedAsync(receivedTask, 100).ConfigureAwait(false);
            await ((IParseBusPublisher)EsbLogger).SendTaskFinishedAsync(receivedTask, currentDate, ServiceOptions.CurrentValue.InstanceName, TaskState.Success, values).ConfigureAwait(false);
        }

    }
}
