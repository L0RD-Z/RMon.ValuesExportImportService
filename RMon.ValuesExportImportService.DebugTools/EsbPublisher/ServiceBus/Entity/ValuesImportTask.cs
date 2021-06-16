using System;
using System.Collections.Generic;
using RMon.ESB.Core.ValuesImportTaskDto;
using RMon.Values.ExportImport.Core;

namespace EsbPublisher.ServiceBus.Entity
{
    class ValuesImportTask : BaseTask, IValuesImportTask
    {
        public ValuesImportTaskParameters Parameters { get; set; }
        public string Name { get; set; }
        public long? IdUser { get; set; }

        public ValuesImportTask(Guid correlationId, IList<ValueInfo> values, long? idUser = null)
        {
            CorrelationId = correlationId;
            Parameters = new()
            {
                Values = values
            };
            IdUser = idUser;
        }

    }
}
