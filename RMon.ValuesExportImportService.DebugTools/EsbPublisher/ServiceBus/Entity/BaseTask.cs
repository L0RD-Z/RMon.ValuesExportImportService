using System;
using RMon.ESB.Core.Common;

namespace EsbPublisher.ServiceBus.Entity
{
    class BaseTask : ITask
    {
        public Guid CorrelationId { get; set; }
        public Guid? ConversationId { get; set; }
        public object TaskId { get; set; }



        public BaseTask()
        {

        }

        public BaseTask(ITask task)
        {
            CorrelationId = task.CorrelationId;
            ConversationId = task.ConversationId;
            TaskId = task.TaskId;
        }
    }
}
