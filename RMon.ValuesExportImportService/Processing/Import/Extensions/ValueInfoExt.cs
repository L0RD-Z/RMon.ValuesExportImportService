using RMon.Values.ExportImport.Core;
using RMon.ValuesExportImportService.Processing.Import.Model;

namespace RMon.ValuesExportImportService.Processing.Import.Extensions
{
    static class ValueInfoExt
    {
        public static Value ToValue(this ValueInfo valueInfo) =>
            new()
            {
                IdDeviceTag = valueInfo.IdTag,
                Datetime = valueInfo.TimeStamp,
                IdQuality = valueInfo.Value.IdQuality,
                ValueBool = valueInfo.Value.ValueBool,
                ValueData = valueInfo.Value.ValueData,
                ValueFloat = valueInfo.Value.ValueFloat,
                ValueInt = valueInfo.Value.ValueInt
            };
    }
}
