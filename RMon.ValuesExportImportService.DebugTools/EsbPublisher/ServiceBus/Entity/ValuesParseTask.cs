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


        public ValuesParseTask(Guid correlationId, string filePath, ValuesParseFileFormatType fileType, Xml80020ParsingParameters taskParams, long? idUser)
            :this(correlationId, filePath, fileType, idUser)
        {
            Parameters.Xml80020Parameters = taskParams;
        }

        public ValuesParseTask(Guid correlationId, string filePath, ValuesParseFileFormatType fileType, Matrix24X31ParsingParameters taskParams, long? idUser)
            : this(correlationId, filePath, fileType, idUser)
        {
            Parameters.Matrix24X31Parameters = taskParams;
        }

        public ValuesParseTask(Guid correlationId, string filePath, ValuesParseFileFormatType fileType, Matrix31X24ParsingParameters taskParams, long? idUser)
            : this(correlationId, filePath, fileType, idUser)
        {
            Parameters.Matrix31X24Parameters = taskParams;
        }

        public ValuesParseTask(Guid correlationId, string filePath, ValuesParseFileFormatType fileType, TableParsingParameters taskParams, long? idUser)
            : this(correlationId, filePath, fileType, idUser)
        {
            Parameters.TableParameters = taskParams;
        }
    }
}
