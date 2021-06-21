using System;
using System.Collections.Generic;
using RMon.Core.Files;
using RMon.ESB.Core.ValuesParseTaskDto;
using RMon.Values.ExportImport.Core;

namespace EsbPublisher.ServiceBus.Entity
{
    class ValuesParseTask : BaseTask, IValuesParseTask
    {
        public ValuesParseTaskParameters Parameters { get; set; }
        public string Name { get; set; }
        public long? IdUser { get; set; }


        public ValuesParseTask(Guid correlationId, string filePath, ValuesParseFileFormatType fileType, long? idUser)
        {
            Parameters = new ValuesParseTaskParameters()
            {
                Files = new List<FileInStorage>
                {
                    new(filePath)
                },
                FileFormatType = fileType
                
            };
            CorrelationId = correlationId;
            IdUser = idUser;
        }
    }
}
