using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RMon.Data.Provider.Esb.Entities.ValuesExportImport;
using RMon.ESB.Core.Common;
using RMon.Globalization.String;
using RMon.Values.ExportImport.Core;
using RMon.ValuesExportImportService.Processing.Parse;
using LogLevel = RMon.ESB.Core.Common.LogLevel;

namespace RMon.ValuesExportImportService.Tests
{
    class ParseTaskLoggerStub : IParseTaskLogger
    {

        private readonly ILogger<ParseTaskLoggerStub> _logger;

        public ParseTaskLoggerStub()
        {
            _logger = new Logger<ParseTaskLoggerStub>(LoggerFactory.Create(c => c.AddDebug()));
        }

        public Task Log(ITask receivedTask, DbValuesExportImportTask dbTask, I18nString msg, LogLevel level, float? progress)
        {
            _logger.LogInformation(progress.HasValue ? $"{msg} Прогресс: {progress}%." : $"{msg}");
            return Task.CompletedTask;
        }

        public Task LogStartedAsync(ITask receivedTask, DbValuesExportImportTask dbTask, I18nString msg)
        {
            _logger.LogInformation($"{msg}");
            return Task.CompletedTask;
        }

        public Task LogAbortedAsync(ITask receivedTask, DbValuesExportImportTask dbTask, I18nString msg)
        {
            _logger.LogInformation($"{msg}");
            return Task.CompletedTask;
        }

        public Task LogFailedAsync(ITask receivedTask, DbValuesExportImportTask dbTask, I18nString msg, Exception ex)
        {
            _logger.LogError(ex, $"{msg}");
            return Task.CompletedTask;
        }

        public Task LogFinishedAsync(ITask receivedTask, DbValuesExportImportTask dbTask, I18nString msg, IList<ValueInfo> values)
        {
            _logger.LogInformation($"{msg}");
            return Task.CompletedTask;
        }
    }
}
