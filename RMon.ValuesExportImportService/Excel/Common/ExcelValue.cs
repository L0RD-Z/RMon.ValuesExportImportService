using System;

namespace RMon.ValuesExportImportService.Excel.Common
{
    class ExcelValue
    {
        public DateTime TimeStamp { get; set; }
        public double Value { get; set; }


        public ExcelValue(DateTime timeStamp, double value)
        {
            TimeStamp = timeStamp;
            Value = value;
        }
    }
}
