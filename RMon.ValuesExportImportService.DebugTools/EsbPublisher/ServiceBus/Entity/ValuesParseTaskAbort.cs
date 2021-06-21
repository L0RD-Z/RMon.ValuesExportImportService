using System;
using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesParseTaskDto;

namespace EsbPublisher.ServiceBus.Entity
{
    class ValuesParseTaskAbort : BaseTask, IValuesParseTaskAbort
    {
        public ValuesParseTaskAbort(ITask task) : base(task)
        {
        }

        public ValuesParseTaskAbort(Guid correlationId) => CorrelationId = correlationId;
    }
}
