using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RMon.Core.CommonTask;
using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesImportTaskDto;
using RMon.ValuesExportImportService.ServiceBus.Common;
using RMon.ValuesExportImportService.ServiceBus.Entity;
using LogLevel = RMon.ESB.Core.Common.LogLevel;

namespace RMon.ValuesExportImportService.ServiceBus.Import
{
    class ImportBusPublisher : IImportBusPublisher
    {
        private readonly IBusProvider _busProvider;
        private readonly ILogger<BusLogger> _busLogger;


        /// <summary>
        /// Конструктор 1
        /// </summary>
        /// <param name="busProvider"></param>
        /// <param name="busLogger"></param>
        public ImportBusPublisher(IBusProvider busProvider, ILogger<BusLogger> busLogger)
        {
            _busProvider = busProvider;
            _busLogger = busLogger;
        }
        

        /// <inheritdoc/>
        public async Task SendTaskStartedAsync(ITask receivedTask, DateTime date, string instanceName)
        {
            var msg = new ValuesImportTaskStarted(receivedTask, date, instanceName);
            await _busProvider.Bus.Publish((IValuesImportTaskStarted)msg).ConfigureAwait(false);
            _busLogger.LogSentTask(msg, typeof(IValuesImportTaskStarted));
        }

        /// <inheritdoc/>
        public async Task SendTaskLogAsync(ITask receivedTask, DateTime date, string message)
        {
            var msg = new ValuesImportTaskLog(receivedTask, date, LogLevel.Info, message, string.Empty);
            await _busProvider.Bus.Publish((IValuesImportTaskLog)msg).ConfigureAwait(false);
            _busLogger.LogSentTask(msg, typeof(IValuesImportTaskLog));
        }

        /// <inheritdoc/>
        public async Task SendTaskLogAsync(ITask receivedTask, DateTime date, LogLevel logLevel, string message)
        {
            var msg = new ValuesImportTaskLog(receivedTask, date, logLevel, message, string.Empty);
            await _busProvider.Bus.Publish((IValuesImportTaskLog)msg).ConfigureAwait(false);
            _busLogger.LogSentTask(msg, typeof(IValuesImportTaskLog));
        }

        /// <inheritdoc/>
        public async Task SendTaskLogAsync(ITask receivedTask, DateTime date, string message, Exception exception)
        {
            var msg = new ValuesImportTaskLog(receivedTask, date, LogLevel.Error, message, exception.StackTrace);
            await _busProvider.Bus.Publish((IValuesImportTaskLog)msg).ConfigureAwait(false);
            _busLogger.LogSentTask(msg, typeof(IValuesImportTaskLog));
        }

        /// <inheritdoc/>
        public async Task SendTaskProgressionChangedAsync(ITask receivedTask, float progress)
        {
            var msg = new ValuesImportTaskProgressionChanged(receivedTask, progress);
            await _busProvider.Bus.Publish((IValuesImportTaskProgressChanged)msg).ConfigureAwait(false);
            _busLogger.LogSentTask(msg, typeof(IValuesImportTaskProgressChanged));
        }

        public async Task SendTaskFinishedAsync(ITask receivedTask, DateTime date, string instanceName, TaskState state)
        {
            var msg = new ValuesImportTaskFinished(receivedTask, date, instanceName, state);
            await _busProvider.Bus.Publish((IValuesImportTaskFinished)msg).ConfigureAwait(false);
            _busLogger.LogSentTask(msg, typeof(IValuesImportTaskFinished));
        }
    }
}
