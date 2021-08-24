using RMon.Data;
using RMon.Values.ExportImport.Core;
using RMon.ValuesExportImportService.Processing.Import.Model;

namespace RMon.ValuesExportImportService.Processing.Extensions
{
    static class ValueInfoExt
    {
        public static Value ToValue(this ValueInfo valueInfo) =>
            new()
            {
                IdDeviceTag = valueInfo.IdTag,
                Datetime = valueInfo.TimeStamp,
                IdQuality = valueInfo.Value.IdQuality,
                ValueBool = valueInfo.Value.ValueBool ?? default,
                ValueData = valueInfo.Value.ValueData ?? default,
                ValueFloat = valueInfo.Value.ValueFloat ?? default,
                ValueInt = valueInfo.Value.ValueInt ?? default
            };

        public static ValueUnion ToValueUnion(this IValue value) =>
            new()
            {
                IdQuality = value.IdQuality,
                ValueBool = value.ValueBool ?? default,
                ValueData = value.ValueData ?? default,
                ValueFloat = value.ValueFloat ?? default,
                ValueInt = value.ValueInt ?? default
            };
    }
}
