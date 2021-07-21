using System;
using System.Threading.Tasks;
using RMon.Data.Provider.Esb.Entities;
using RMon.ESB.Core.Common;
using RMon.Globalization;
using RMon.Globalization.String;
using RMon.ValuesExportImportService.Processing.Common;

namespace RMon.ValuesExportImportService.Processing
{
    public class ProcessingContext<T> : IProcessingContext 
        where T : DbTask
    {
        protected readonly IBaseTaskLogger<T> TaskLogger;
        protected readonly ITask Task;
        protected readonly T DbTask;

        public long IdUser { get; set; }
        public IGlobalizationProvider GlobalizationProvider { get; set; }

        public ProcessingContext(ITask task, T dbTask, IBaseTaskLogger<T> taskLogger, long idUser)
        {
            IdUser = idUser;
            TaskLogger = taskLogger;
            Task = task;
            DbTask = dbTask;
        }

        public Task LogStarted(I18nString msg) => TaskLogger.LogStartedAsync(Task, DbTask, msg);

        public Task LogAborted(I18nString msg) => TaskLogger.LogAbortedAsync(Task, DbTask, msg);

        public Task LogFailed(I18nString msg, Exception ex) => TaskLogger.LogFailedAsync(Task, DbTask, msg, ex);

        public Task Log(I18nString msg, LogLevel logLevel, float? progress) => TaskLogger.Log(Task, DbTask, msg, logLevel, progress);
    }
}
