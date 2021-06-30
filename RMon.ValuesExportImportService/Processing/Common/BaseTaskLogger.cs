using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RMon.Configuration.Options;
using RMon.Core.CommonTask;
using RMon.Data.Provider.Esb.Backend;
using RMon.Data.Provider.Esb.Entities;
using RMon.ESB.Core.Common;
using RMon.Globalization.String;
using RMon.ValuesExportImportService.Common;
using RMon.ValuesExportImportService.Data;
using RMon.ValuesExportImportService.ServiceBus.Common;
using LogLevel = RMon.ESB.Core.Common.LogLevel;

namespace RMon.ValuesExportImportService.Processing.Common
{
    public abstract class BaseTaskLogger<T> : IBaseTaskLogger<T> where T : DbTask
    {
        protected readonly IOptionsMonitor<Service> ServiceOptions;
        protected readonly IRepositoryFactoryConfigurator RepositoryFactoryConfigurator;
        protected readonly IDataRepository DataRepository;

        protected readonly ILogger FileLogger;
        protected IRepository DbLogger => RepositoryFactoryConfigurator.TaskRepositoryCreate();
        protected readonly IBusPublisher EsbLogger;

        //private ITask _receivedTask;
        //protected T DbTask { get; private set; }

        /// <summary>
        /// Конструктор 1
        /// </summary>
        /// <param name="serviceOptions">Опции сервиса</param>
        /// <param name="repositoryFactoryConfigurator">Конфигуратор репозиториев</param>
        /// <param name="dataRepository">Репозиторий данных</param>
        /// <param name="fileLogger">Логгер для записи логов в файл</param>
        /// <param name="esbLogger">Логгер для передачи логов по ESB</param>
        protected BaseTaskLogger(
            IOptionsMonitor<Service> serviceOptions, 
            IRepositoryFactoryConfigurator repositoryFactoryConfigurator, 
            IDataRepository dataRepository, 
            ILogger fileLogger, 
            IBusPublisher esbLogger)
        {
            ServiceOptions = serviceOptions;
            RepositoryFactoryConfigurator = repositoryFactoryConfigurator;
            DataRepository = dataRepository;
            
            FileLogger = fileLogger;
            EsbLogger = esbLogger;
        }

        
        /// <inheritdoc />
        public async Task Log(ITask receivedTask, T dbTask, I18nString msg, LogLevel level, float? progress)
        {
            var currentDate = await DataRepository.GetDateAsync().ConfigureAwait(false);
            
            if (progress.HasValue)
            {
                dbTask.Progress = progress.Value;
                dbTask.UpdateDate = currentDate;
                await DbLogger.UpdateTaskProgressAsync<T>(dbTask.Id, progress.Value).ConfigureAwait(false);
                await EsbLogger.SendTaskProgressionChangedAsync(receivedTask, progress.Value).ConfigureAwait(false);
            }

            var dbLogLevel = level switch
            {
                LogLevel.Info => DbTaskLogLevel.Info,
                LogLevel.Warning => DbTaskLogLevel.Warning,
                LogLevel.Error => DbTaskLogLevel.Error,
                _ => DbTaskLogLevel.Error
            };

            FileLogger.LogInformation($"{receivedTask.CorrelationId}: {msg}");
            await DbLogger.AddToTaskLogAsync<T>(dbTask.Id, currentDate, dbLogLevel, msg.ToJsonString()).ConfigureAwait(false);
            await EsbLogger.SendTaskLogAsync(receivedTask, currentDate, level, msg.ToString()).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task LogStartedAsync(ITask receivedTask, T dbTask, I18nString msg)
        {
            FileLogger.LogInformation($"{receivedTask.CorrelationId}: {msg}");
            var currentDate = await DataRepository.GetDateAsync().ConfigureAwait(false);

            dbTask.State = TaskState.Executing;
            dbTask.CreateDate = currentDate;
            dbTask.StartDate = currentDate;
            dbTask.UpdateDate = currentDate;
            await DbLogger.CreateTaskAsync(dbTask).ConfigureAwait(false);
            receivedTask.TaskId = dbTask.Id;

            await DbLogger.AddToTaskLogAsync<T>(dbTask.Id, currentDate, DbTaskLogLevel.Info, msg.ToJsonString()).ConfigureAwait(false);

            await EsbLogger.SendTaskStartedAsync(receivedTask, currentDate, ServiceOptions.CurrentValue.InstanceName).ConfigureAwait(false);
            await EsbLogger.SendTaskLogAsync(receivedTask, currentDate, msg.ToString()).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task LogAbortedAsync(ITask receivedTask, T dbTask, I18nString msg)
        {
            FileLogger.LogInformation($"{receivedTask.CorrelationId}: {msg}");
            var currentDate = await DataRepository.GetDateAsync().ConfigureAwait(false);

            dbTask.State = TaskState.Aborted;
            dbTask.FinishDate = currentDate;
            dbTask.UpdateDate = currentDate;
            await DbLogger.UpdateTaskAsync(dbTask).ConfigureAwait(false);

            await DbLogger.AddToTaskLogAsync<T>(dbTask.Id, currentDate, DbTaskLogLevel.Info, msg.ToJsonString()).ConfigureAwait(false);

            await EsbLogger.SendTaskLogAsync(receivedTask, currentDate, msg.ToString()).ConfigureAwait(false);
            await EsbLogger.SendTaskFinishedAsync(receivedTask, currentDate, ServiceOptions.CurrentValue.InstanceName, TaskState.Aborted).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task LogFailedAsync(ITask receivedTask, T dbTask, I18nString msg, Exception ex)
        {
            FileLogger.LogError(ex, $"{receivedTask.CorrelationId}: {msg}");
            var currentDate = await DataRepository.GetDateAsync().ConfigureAwait(false);

            dbTask.State = TaskState.Failed;
            dbTask.FinishDate = currentDate;
            dbTask.UpdateDate = currentDate;
            await DbLogger.UpdateTaskAsync(dbTask).ConfigureAwait(false);
            await DbLogger.AddToTaskLogAsync<T>(dbTask.Id, currentDate, DbTaskLogLevel.Error, msg.ToJsonString(), ConcatAllExceptionMessage(ex, true)).ConfigureAwait(false);

            await EsbLogger.SendTaskLogAsync(receivedTask, currentDate, msg.ToString(), ex).ConfigureAwait(false);
            await EsbLogger.SendTaskFinishedAsync(receivedTask, currentDate, ServiceOptions.CurrentValue.InstanceName, TaskState.Failed).ConfigureAwait(false);
        }
        
        private string ConcatAllExceptionMessage(Exception ex, bool withStackTrace)
        {
            var str = $" => {ex.GetType().Name}:{ex.Message}";
            if (withStackTrace)
                str = $"{str}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}";
            return ex.InnerException == null ? $"{str}{Environment.NewLine}" : str + ConcatAllExceptionMessage(ex.InnerException, withStackTrace);
        }

        //Todo переделать логирование так, чтобы из вложенных исключений вытаскивался I18nString с помощью этого метода, как в DCom
        private I18nString ConcatExceptionMessage(Exception ex) => ex.ConcatExceptionMessage();
    }
}
