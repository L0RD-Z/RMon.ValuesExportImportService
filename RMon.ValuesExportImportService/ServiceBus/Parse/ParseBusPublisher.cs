using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RMon.Core.CommonTask;
using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesParseTaskDto;
using RMon.Values.ExportImport.Core;
using RMon.ValuesExportImportService.ServiceBus.Common;
using RMon.ValuesExportImportService.ServiceBus.Entity;
using LogLevel = RMon.ESB.Core.Common.LogLevel;

namespace RMon.ValuesExportImportService.ServiceBus.Parse
{
    class ParseBusPublisher : IParseBusPublisher
    {
        private readonly IBusProvider _busProvider;
        private readonly ILogger<BusLogger> _busLogger;


        /// <summary>
        /// Конструктор 1
        /// </summary>
        /// <param name="busProvider"></param>
        /// <param name="busLogger"></param>
        public ParseBusPublisher(IBusProvider busProvider, ILogger<BusLogger> busLogger)
        {
            _busProvider = busProvider;
            _busLogger = busLogger;
        }
        

        /// <inheritdoc/>
        public async Task SendTaskStartedAsync(ITask receivedTask, DateTime date, string instanceName)
        {
            var msg = new ValuesParseTaskStarted(receivedTask, date, instanceName);
            await _busProvider.Bus.Publish((IValuesParseTaskStarted)msg).ConfigureAwait(false);
            _busLogger.LogSentTask(msg, typeof(IValuesParseTaskStarted));
        }

        /// <inheritdoc/>
        public async Task SendTaskLogAsync(ITask receivedTask, DateTime date, string message)
        {
            var msg = new ValuesParseTaskLog(receivedTask, date, LogLevel.Info, message, string.Empty);
            await _busProvider.Bus.Publish((IValuesParseTaskLog)msg).ConfigureAwait(false);
            _busLogger.LogSentTask(msg, typeof(IValuesParseTaskLog));
        }

        /// <inheritdoc/>
        public async Task SendTaskLogAsync(ITask receivedTask, DateTime date, LogLevel logLevel, string message)
        {
            var msg = new ValuesParseTaskLog(receivedTask, date, logLevel, message, string.Empty);
            await _busProvider.Bus.Publish((IValuesParseTaskLog)msg).ConfigureAwait(false);
            _busLogger.LogSentTask(msg, typeof(IValuesParseTaskLog));
        }

        /// <inheritdoc/>
        public async Task SendTaskLogAsync(ITask receivedTask, DateTime date, string message, Exception exception)
        {
            var msg = new ValuesParseTaskLog(receivedTask, date, LogLevel.Error, message, exception.StackTrace);
            await _busProvider.Bus.Publish((IValuesParseTaskLog)msg).ConfigureAwait(false);
            _busLogger.LogSentTask(msg, typeof(IValuesParseTaskLog));
        }

        /// <inheritdoc/>
        public async Task SendTaskProgressionChangedAsync(ITask receivedTask, float progress)
        {
            var msg = new ValuesParseTaskProgressionChanged(receivedTask, progress);
            await _busProvider.Bus.Publish((IValuesParseTaskProgressChanged)msg).ConfigureAwait(false);
            _busLogger.LogSentTask(msg, typeof(IValuesParseTaskProgressChanged));
        }

        /// <inheritdoc/>
        public async Task SendTaskFinishedAsync(ITask receivedTask, DateTime date, string instanceName, TaskState state)
        {
            var msg = new ValuesParseTaskFinished(receivedTask, date, instanceName, state);
            await _busProvider.Bus.Publish((IValuesParseTaskFinished)msg).ConfigureAwait(false);
            _busLogger.LogSentTask(msg, typeof(IValuesParseTaskFinished));
        }

        /// <inheritdoc/>
        public async Task SendTaskFinishedAsync(ITask receivedTask, DateTime date, string instanceName, TaskState state, IList<ValueInfo> values)
        {
            var msg = new ValuesParseTaskFinished(receivedTask, date, instanceName, state, values);
            await _busProvider.Bus.Publish((IValuesParseTaskFinished)msg).ConfigureAwait(false);
            _busLogger.LogSentTask(msg, typeof(IValuesParseTaskFinished));
        }
    }
}
