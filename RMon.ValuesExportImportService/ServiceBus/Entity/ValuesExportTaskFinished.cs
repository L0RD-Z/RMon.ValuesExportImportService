using System;
using System.Collections.Generic;
using RMon.Core.CommonTask;
using RMon.Core.Files;
using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesExportTaskDto;
using RMon.Values.ExportImport.Core;

namespace RMon.ValuesExportImportService.ServiceBus.Entity
{
    /// <inheritdoc cref="IValuesExportTaskFinished"/>
    class ValuesExportTaskFinished : BaseTask, IValuesExportTaskFinished
    {
        public DateTime DateTime { get; set; }
        public string InstanceName { get; set; }
        public TaskState State { get; set; }

        public ValuesExportTaskResults Results { get; set; }

        public ValuesExportTaskFinished(ITask task, DateTime dateTime, string instanceName, TaskState state, IList<FileInStorage> files)
            :base(task)
        {
            DateTime = dateTime;
            InstanceName = instanceName;
            State = state;
            Results = new ValuesExportTaskResults() {Files = files};
        }

        public ValuesExportTaskFinished(ITask task, DateTime dateTime, string instanceName, TaskState state) 
            : this(task, dateTime, instanceName, state, null)
        {
        }
    }
}
