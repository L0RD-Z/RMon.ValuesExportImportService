using System;
using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesParseTaskDto;

namespace RMon.ValuesExportImportService.ServiceBus.Entity
{
    /// <inheritdoc cref="IValuesParseTaskStarted"/>
    class ValuesParseTaskStarted : BaseTask, IValuesParseTaskStarted
    {
        public DateTime DateTime { get; set; }
        public string InstanceName { get; set; }



        public ValuesParseTaskStarted(ITask task, DateTime dateTime, string instanceName)
            :base(task)
        {
            ConversationId = task.ConversationId;
            DateTime = dateTime;
            InstanceName = instanceName;
        }
    }
}
