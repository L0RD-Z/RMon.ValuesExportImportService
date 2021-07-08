using System;
using RMon.Data;

namespace RMon.ValuesExportImportService.Processing.Import.Model
{
    class Value : IValue
    {
        public long IdDeviceTag { get; set; }
        public DateTime Datetime { get; set; }
        public string IdQuality { get; set; }
        public double? ValueFloat { get; set; }
        public int? ValueInt { get; set; }
        public string ValueData { get; set; }
        public bool? ValueBool { get; set; }
    }
}
