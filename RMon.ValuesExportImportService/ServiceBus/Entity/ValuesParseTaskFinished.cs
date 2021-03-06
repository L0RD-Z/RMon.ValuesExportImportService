using System;
using System.Collections.Generic;
using RMon.Core.CommonTask;
using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesImportTaskDto;
using RMon.ESB.Core.ValuesParseTaskDto;
using RMon.Values.ExportImport.Core;


namespace RMon.ValuesExportImportService.ServiceBus.Entity
{
    /// <inheritdoc cref="IValuesImportTaskFinished"/>
    class ValuesParseTaskFinished : BaseTask, IValuesParseTaskFinished
    {
        public DateTime DateTime { get; set; }
        public string InstanceName { get; set; }
        public TaskState State { get; set; }

        public ValuesParseTaskResults Results { get; set; }

        public ValuesParseTaskFinished(ITask task, DateTime dateTime, string instanceName, TaskState state, IList<ValueInfo> values)
            :base(task)
        {
            DateTime = dateTime;
            InstanceName = instanceName;
            State = state;
            Results = new ValuesParseTaskResults
            {
                Values = values
            };
        }

        public ValuesParseTaskFinished(ITask task, DateTime dateTime, string instanceName, TaskState state) : 
            this(task, dateTime, instanceName, state, null)
        {
        }
    }
}
