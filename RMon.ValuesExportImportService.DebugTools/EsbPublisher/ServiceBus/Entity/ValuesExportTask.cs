using System;
using System.Collections.Generic;
using RMon.ESB.Core.ValuesExportTaskDto;
using RMon.Values.ExportImport.Core;

namespace EsbPublisher.ServiceBus.Entity
{
    class ValuesExportTask: BaseTask, IValuesExportTask
    {
        public ValuesExportTaskParameters Parameters { get; set; }
        public string Name { get; set; }
        public long? IdUser { get; set; }


        public ValuesExportTask(Guid correlationId, DateTime dateTimeStart, DateTime dateTimeEnd, IList<long> idLogicDevices, IList<string> tagTypeCodes, IList<string> propertyCodes, long? idUser = null)
        {
            CorrelationId = correlationId;
            Parameters = new()
            {
                
                DateTimeStart = dateTimeStart,
                DateTimeEnd = dateTimeEnd,
                IdLogicDevices = idLogicDevices,
                TagTypeCodes = tagTypeCodes,
                PropertyCodes = propertyCodes
            };
            IdUser = idUser;
        }
    }
}
