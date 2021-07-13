using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RMon.Values.ExportImport.Core;
using RMon.ValuesExportImportService.Configuration;

namespace RMon.ValuesExportImportService.Processing.Common
{
    class ValuesLogger : IValuesLogger
    {
        private readonly IOptionsMonitor<ValuesLoggingOptions> _loggingOptions;
        private readonly ILogger _logger;

        public ValuesLogger(IOptionsMonitor<ValuesLoggingOptions> loggingOptions, ILogger<ValuesLogger> logger)
        {
            _loggingOptions = loggingOptions;
            _logger = logger;
        }


        
        /// <inheritdoc />
        public void LogReceivedValues(Guid correlationId, IList<ValueInfo> values) => LogValues(correlationId, values, TaskActions.Received);

        /// <inheritdoc />
        public void LogSendValues(Guid correlationId, IList<ValueInfo> values) => LogValues(correlationId, values, TaskActions.Sent);


        private void LogValues(Guid correlationId, IList<ValueInfo> values, TaskActions action)
        {
            if (_loggingOptions.CurrentValue.LogMessages)
            {
                var valuesGroup = values.GroupBy(t => t.IdTag).OrderBy(t => t.Key);

                var logMessages = new StringBuilder();
                logMessages.AppendLine($"Задача {correlationId}, {(action == TaskActions.Sent ? "отправлено:" : "получено:")}");
                foreach (var tag in valuesGroup)
                {
                    var tagValues = tag.OrderBy(t => t.TimeStamp);
                    logMessages.AppendLine($"Значения тега (id = {tag.Key}):");
                    foreach (var value in tagValues) 
                        logMessages.AppendLine($"  {value.TimeStamp} - \"{nameof(value.Value)}\" = {value.Value?.ValueFloat}, \"{nameof(value.CurrentValue)}\" = {value.CurrentValue?.ValueFloat}, \"{nameof(value.Rewrite)}\" = {value.Rewrite}");
                }

                logMessages.AppendLine();
                _logger.LogInformation(logMessages.ToString());
            }
        }

        private enum TaskActions
        {
            Sent,
            Received
        }
    }
}
