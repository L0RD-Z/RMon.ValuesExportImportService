using System;
using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesExportTaskDto;

namespace RMon.ValuesExportImportService.ServiceBus.Entity
{
    /// <inheritdoc cref="IValuesExportTaskStarted"/>
    class ValuesExportTaskStarted : BaseTask, IValuesExportTaskStarted
    {
        public DateTime DateTime { get; set; }
        public string InstanceName { get; set; }


        public ValuesExportTaskStarted(ITask task, DateTime dateTime, string instanceName)
            :base(task)
        {
            ConversationId = task.ConversationId;
            DateTime = dateTime;
            InstanceName = instanceName;
        }
    }
}
