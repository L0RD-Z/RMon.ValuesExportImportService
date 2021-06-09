using System;
using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesExportTaskDto;

namespace EsbPublisher.ServiceBus.Entity
{
    class ValuesExportTaskAbort : BaseTask, IValuesExportTaskAbort
    {
        public ValuesExportTaskAbort(ITask task) : base(task)
        {
        }

        public ValuesExportTaskAbort(Guid correlationId) => CorrelationId = correlationId;
    }
}
