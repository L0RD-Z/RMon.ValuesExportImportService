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

        public ValuesParseTask(Guid correlationId, string filePath, ValuesParseFileFormatType fileType, bool useTransformationRatio, long? idUser)
        {
            Parameters = new ValuesParseTaskParameters()
            {
                Files = new List<FileInStorage>
                {
                    new(filePath)
                },
                FileFormatType = fileType,
                UseTransformationRatio = useTransformationRatio
            };
            CorrelationId = correlationId;
            IdUser = idUser;
        }


        public ValuesParseTask(Guid correlationId, string filePath, ValuesParseFileFormatType fileType, bool useTransformationRatio, Xml80020ParsingParameters taskParams, long? idUser)
            :this(correlationId, filePath, fileType, useTransformationRatio, idUser)
        {
            Parameters.Xml80020Parameters = taskParams;
        }

        public ValuesParseTask(Guid correlationId, string filePath, ValuesParseFileFormatType fileType, bool useTransformationRatio, Matrix24X31ParsingParameters taskParams, long? idUser)
            : this(correlationId, filePath, fileType, useTransformationRatio, idUser)
        {
            Parameters.Matrix24X31Parameters = taskParams;
        }

        public ValuesParseTask(Guid correlationId, string filePath, ValuesParseFileFormatType fileType, bool useTransformationRatio, Matrix31X24ParsingParameters taskParams, long? idUser)
            : this(correlationId, filePath, fileType, useTransformationRatio, idUser)
        {
            Parameters.Matrix31X24Parameters = taskParams;
        }

        public ValuesParseTask(Guid correlationId, string filePath, ValuesParseFileFormatType fileType, bool useTransformationRatio, TableParsingParameters taskParams, long? idUser)
            : this(correlationId, filePath, fileType, useTransformationRatio, idUser)
        {
            Parameters.TableParameters = taskParams;
        }
    }
}
