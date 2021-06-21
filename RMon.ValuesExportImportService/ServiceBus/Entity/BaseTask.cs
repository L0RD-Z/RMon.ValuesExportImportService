using System;
using RMon.ESB.Core.Common;

namespace RMon.ValuesExportImportService.ServiceBus.Entity
{
    class BaseTask:ITask
    {
        public Guid CorrelationId { get; set; }
        public Guid? ConversationId { get; set; }
        public object TaskId { get; set; }


        public BaseTask(ITask task)
        {
            CorrelationId = task.CorrelationId;
            TaskId = task.TaskId;
        }
    }
}
