using System;
using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesImportTaskDto;

namespace RMon.ValuesExportImportService.ServiceBus.Entity
{
    /// <inheritdoc cref="IValuesImportTaskStarted"/>
    class ValuesImportTaskStarted : BaseTask, IValuesImportTaskStarted
    {
        public DateTime DateTime { get; set; }
        public string InstanceName { get; set; }



        public ValuesImportTaskStarted(ITask task, DateTime dateTime, string instanceName)
            :base(task)
        {
            ConversationId = task.ConversationId;
            DateTime = dateTime;
            InstanceName = instanceName;
        }
    }
}
