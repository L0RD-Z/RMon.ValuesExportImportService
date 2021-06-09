using System;
using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesImportTaskDto;

namespace EsbPublisher.ServiceBus.Entity
{
    class ValuesImportTaskAbort : BaseTask, IValuesImportTaskAbort
    {
        public ValuesImportTaskAbort(ITask task) : base(task)
        {
        }

        public ValuesImportTaskAbort(Guid correlationId) => CorrelationId = correlationId;
    }
}
