using System;
using System.Collections.Generic;
using RMon.Core.Files;
using RMon.ESB.Core.ValuesParseTaskDto;
using RMon.Values.ExportImport.Core;
using RMon.Values.ExportImport.Core.FileFormatParameters;

namespace EsbPublisher.ServiceBus.Entity
{
    class ValuesParseTask : BaseTask, IValuesParseTask
    {
        public ValuesParseTaskParameters Parameters { get; set; }
        public string Name { get; set; }
        public long? IdUser { get; set; }


        public ValuesParseTask(Guid correlationId, string filePath, ValuesParseFileFormatType fileType, Xml80020ParsingParameters taskParams, long? idUser)
        {
            Parameters = new ValuesParseTaskParameters()
            {
                Files = new List<FileInStorage>
                {
                    new(filePath)
                },
                FileFormatType = fileType,
                Xml80020Parameters = taskParams

            };
            CorrelationId = correlationId;
            IdUser = idUser;
        }
    }
}
