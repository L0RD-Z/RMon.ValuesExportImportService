using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RMon.Data.Provider.Esb.Entities.ValuesExportImport;
using RMon.ESB.Core.Common;
using RMon.Globalization.String;
using RMon.Values.ExportImport.Core;
using RMon.ValuesExportImportService.Processing.Parse;
using LogLevel = RMon.ESB.Core.Common.LogLevel;

namespace RMon.ValuesExportImportService.Tests.Parse80020
{
    class ParseTaskLoggerStub : IParseTaskLogger
    {
        public ParseTaskLoggerStub() 
            
        {
        }

        public Task Log(ITask receivedTask, DbValuesExportImportTask dbTask, I18nString msg, LogLevel level, float? progress)
        {
            return Task.CompletedTask;
        }

        public Task LogStartedAsync(ITask receivedTask, DbValuesExportImportTask dbTask, I18nString msg)
        {
            return Task.CompletedTask;
        }

        public Task LogAbortedAsync(ITask receivedTask, DbValuesExportImportTask dbTask, I18nString msg)
        {
            return Task.CompletedTask;
        }

        public Task LogFailedAsync(ITask receivedTask, DbValuesExportImportTask dbTask, I18nString msg, Exception ex)
        {
            return Task.CompletedTask;
        }

        public Task LogFinishedAsync(ITask receivedTask, DbValuesExportImportTask dbTask, I18nString msg, IList<ValueInfo> values)
        {
            return Task.CompletedTask;
        }
    }
}
