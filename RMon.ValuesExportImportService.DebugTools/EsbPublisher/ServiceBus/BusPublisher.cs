using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsbPublisher.ServiceBus.Entity;
using MassTransit;
using RMon.ESB.Core.ValuesExportTaskDto;
using RMon.ESB.Core.ValuesImportTaskDto;
using RMon.ESB.Core.ValuesParseTaskDto;
using RMon.Values.ExportImport.Core;
using RMon.Values.ExportImport.Core.FileFormatParameters;

namespace EsbPublisher.ServiceBus
{
    public class BusPublisher
    {
        private readonly IBusControl _bus;

        public BusPublisher(IBusControl bus)
        {
            _bus = bus;
        }


        public Task SendExportTaskAsync(Guid correlationId, DateTime startDate, DateTime endDate, IList<long> idLogicDevices, IList<string> tagTypeCodes, IList<string> propertyCodes, long idUser)
        {
            var sendTask = new ValuesExportTask(correlationId, startDate, endDate, idLogicDevices, tagTypeCodes, propertyCodes, idUser);
            return _bus.Publish((IValuesExportTask)sendTask);
        }


        public Task SendExportTaskAbortAsync(Guid correlationId)
        {
            var sendTask = new ValuesExportTaskAbort(correlationId);
            return _bus.Publish((IValuesExportTaskAbort)sendTask);
        }

        public Task SendParseTaskAsync(Guid correlationId, string filePath, ValuesParseFileFormatType fileType, bool useTransformationRatio,  Xml80020ParsingParameters taskParams, long idUser)
        {
            var sendTask = new ValuesParseTask(correlationId, filePath, fileType, useTransformationRatio, taskParams, idUser);
            return _bus.Publish((IValuesParseTask)sendTask);
        }

        public Task SendParseTaskAsync(Guid correlationId, string filePath, ValuesParseFileFormatType fileType, bool useTransformationRatio, Matrix24X31ParsingParameters taskParams, long idUser)
        {
            var sendTask = new ValuesParseTask(correlationId, filePath, fileType, useTransformationRatio, taskParams, idUser);
            return _bus.Publish((IValuesParseTask)sendTask);
        }

        public Task SendParseTaskAsync(Guid correlationId, string filePath, ValuesParseFileFormatType fileType, bool useTransformationRatio, Matrix31X24ParsingParameters taskParams, long idUser)
        {
            var sendTask = new ValuesParseTask(correlationId, filePath, fileType, useTransformationRatio, taskParams, idUser);
            return _bus.Publish((IValuesParseTask)sendTask);
        }

        public Task SendParseTaskAsync(Guid correlationId, string filePath, ValuesParseFileFormatType fileType, bool useTransformationRatio, TableParsingParameters taskParams, long idUser)
        {
            var sendTask = new ValuesParseTask(correlationId, filePath, fileType, useTransformationRatio, taskParams, idUser);
            return _bus.Publish((IValuesParseTask)sendTask);
        }

        public Task SendParseTaskAsync(Guid correlationId, string filePath, ValuesParseFileFormatType fileType, bool useTransformationRatio, long idUser)
        {
            var sendTask = new ValuesParseTask(correlationId, filePath, fileType, useTransformationRatio, idUser);
            return _bus.Publish((IValuesParseTask)sendTask);
        }


        public Task SendParseTaskAbortAsync(Guid correlationId)
        {
            var sendTask = new ValuesParseTaskAbort(correlationId);
            return _bus.Publish((IValuesParseTaskAbort)sendTask);
        }


        public Task SendImportTaskAsync(Guid correlationId, IList<ValueInfo> values, long idUser)
        {
            var sendTask = new ValuesImportTask(correlationId, values, idUser);
            return _bus.Publish((IValuesImportTask)sendTask);
        }
        

        public Task SendImportTaskAbortAsync(Guid correlationId)
        {
            var sendTask = new ValuesImportTaskAbort(correlationId);
            return _bus.Publish((IValuesImportTaskAbort)sendTask);
        }
    }
}
