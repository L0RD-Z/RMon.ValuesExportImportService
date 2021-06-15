using System;
using RMon.Core.CommonTask;
using RMon.Data.Provider.Esb.Entities.ValuesExportImport;
using RMon.ESB.Core.ValuesExportTaskDto;
using RMon.ESB.Core.ValuesImportTaskDto;
using RMon.ESB.Core.ValuesParseTaskDto;


namespace RMon.ValuesExportImportService.Processing.Common
{
    static class TaskExtensions
    {
        /// <summary>
        /// Конвертирует <see cref="IValuesExportTask"/> в <see cref="DbValuesExportImportTask"/>
        /// </summary>
        /// <param name="task"></param>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        public static DbValuesExportImportTask ToDbTask(this IValuesExportTask task, string instanceName) =>
            new()
            {
                State = TaskState.NotYetExecuted,
                CreateDate = DateTime.Now,
                StartDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                CorrelationId = task.CorrelationId,
                Name = task.Name,
                Progress = 0,
                IdUser = task.IdUser,
                OperationType = OperationTypes.Export,
                ExportParameters = task.Parameters,
                DirectorInstance = instanceName,
                ExecutorInstance = instanceName
            };

        /// <summary>
        /// Конвертирует <see cref="IValuesImportTask"/> в <see cref="DbValuesExportImportTask"/>
        /// </summary>
        /// <param name="task"></param>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        public static DbValuesExportImportTask ToDbTask(this IValuesImportTask task, string instanceName) =>
            new()
            {
                State = TaskState.NotYetExecuted,
                CreateDate = DateTime.Now,
                StartDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                CorrelationId = task.CorrelationId,
                Name = task.Name,
                Progress = 0,
                IdUser = task.IdUser,
                OperationType = OperationTypes.Export,
                ImportParameters = task.Parameters,
                DirectorInstance = instanceName,
                ExecutorInstance = instanceName
            };

        /// <summary>
        /// Конвертирует <see cref="IValuesParseTask"/> в <see cref="DbValuesExportImportTask"/>
        /// </summary>
        /// <param name="task"></param>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        public static DbValuesExportImportTask ToDbTask(this IValuesParseTask task, string instanceName) =>
            new()
            {
                State = TaskState.NotYetExecuted,
                CreateDate = DateTime.Now,
                StartDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                CorrelationId = task.CorrelationId,
                Name = task.Name,
                Progress = 0,
                IdUser = task.IdUser,
                OperationType = OperationTypes.Export,
                ParseParameters = task.Parameters,
                DirectorInstance = instanceName,
                ExecutorInstance = instanceName
            };


    }
}
