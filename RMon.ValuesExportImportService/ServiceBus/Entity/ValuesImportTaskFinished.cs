using System;
using RMon.Core.CommonTask;
using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesImportTaskDto;
using RMon.Values.ExportImport.Core;

namespace RMon.ValuesExportImportService.ServiceBus.Entity
{
    /// <inheritdoc cref="IValuesImportTaskFinished"/>
    class ValuesImportTaskFinished : BaseTask, IValuesImportTaskFinished
    {
        public DateTime DateTime { get; set; }
        public string InstanceName { get; set; }
        public TaskState State { get; set; }

        public ValuesImportTaskResults Results { get; set; }

        public ValuesImportTaskFinished(ITask task, DateTime dateTime, string instanceName, TaskState state)
            :base(task)
        {
            DateTime = dateTime;
            InstanceName = instanceName;
            State = state;
            Results = new ValuesImportTaskResults();
        }

        
    }
}
