using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RMon.Core.CommonTask;
using RMon.Core.Files;
using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesExportTaskDto;
using RMon.ValuesExportImportService.ServiceBus.Common;
using RMon.ValuesExportImportService.ServiceBus.Entity;
using LogLevel = RMon.ESB.Core.Common.LogLevel;

namespace RMon.ValuesExportImportService.ServiceBus.Export
{
    class ExportBusPublisher : IExportBusPublisher
    {
        private readonly IBusProvider _busProvider;
        private readonly ILogger<BusLogger> _busLogger;


        /// <summary>
        /// Конструктор 1
        /// </summary>
        /// <param name="busProvider"></param>
        /// <param name="busLogger"></param>
        public ExportBusPublisher(IBusProvider busProvider, ILogger<BusLogger> busLogger)
        {
            _busProvider = busProvider;
            _busLogger = busLogger;
        }



        /// <inheritdoc/>
        public async Task SendTaskStartedAsync(ITask receivedTask, DateTime date, string instanceName)
        {
            var msg = new ValuesExportTaskStarted(receivedTask, date, instanceName);
            await _busProvider.Bus.Publish((IValuesExportTaskStarted) msg).ConfigureAwait(false);
            _busLogger.LogSentTask(msg, typeof(IValuesExportTaskStarted));
        }

        /// <inheritdoc/>
        public async Task SendTaskLogAsync(ITask receivedTask, DateTime date, string message)
        {
            var msg = new ValuesExportTaskLog(receivedTask, date, LogLevel.Info, message, string.Empty);
            await _busProvider.Bus.Publish((IValuesExportTaskLog)msg).ConfigureAwait(false);
            _busLogger.LogSentTask(msg, typeof(IValuesExportTaskLog));
        }

        /// <inheritdoc/>
        public async Task SendTaskLogAsync(ITask receivedTask, DateTime date, LogLevel logLevel, string message)
        {
            var msg = new ValuesExportTaskLog(receivedTask, date, logLevel, message, string.Empty);
            await _busProvider.Bus.Publish((IValuesExportTaskLog)msg).ConfigureAwait(false);
            _busLogger.LogSentTask(msg, typeof(IValuesExportTaskLog));
        }

        /// <inheritdoc/>
        public async Task SendTaskLogAsync(ITask receivedTask, DateTime date, string message, Exception exception)
        {
            var msg = new ValuesExportTaskLog(receivedTask, date, LogLevel.Error, message, exception.StackTrace);
            await _busProvider.Bus.Publish((IValuesExportTaskLog)msg).ConfigureAwait(false);
            _busLogger.LogSentTask(msg, typeof(IValuesExportTaskLog));
        }

        /// <inheritdoc/>
        public async Task SendTaskProgressionChangedAsync(ITask receivedTask, float progress)
        {
            var msg = new ValuesExportTaskProgressionChanged(receivedTask, progress);
            await _busProvider.Bus.Publish((IValuesExportTaskProgressChanged)msg).ConfigureAwait(false);
            _busLogger.LogSentTask(msg, typeof(IValuesExportTaskProgressChanged));
        }

        /// <inheritdoc/>
        public async Task SendTaskFinishedAsync(ITask receivedTask, DateTime date, string instanceName, TaskState state)
        {
            var msg = new ValuesExportTaskFinished(receivedTask, date, instanceName, state);
            await _busProvider.Bus.Publish((IValuesExportTaskFinished)msg).ConfigureAwait(false);
            _busLogger.LogSentTask(msg, typeof(IValuesExportTaskFinished));
        }

        /// <inheritdoc/>
        public async Task SendTaskFinishedAsync(ITask receivedTask, DateTime date, string instanceName, TaskState state, IList<FileInStorage> files)
        {
            var msg = new ValuesExportTaskFinished(receivedTask, date, instanceName, state, files);
            await _busProvider.Bus.Publish((IValuesExportTaskFinished)msg).ConfigureAwait(false);
            _busLogger.LogSentTask(msg, typeof(IValuesExportTaskFinished));
        }
    }
}
