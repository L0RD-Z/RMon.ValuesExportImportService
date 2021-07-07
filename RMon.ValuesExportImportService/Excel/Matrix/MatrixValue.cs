using System;

namespace RMon.ValuesExportImportService.Excel.Matrix
{
    class MatrixValue
    {
        public DateTime TimeStamp { get; set; }
        public double Value { get; set; }


        public MatrixValue(DateTime timeStamp, double value)
        {
            TimeStamp = timeStamp;
            Value = value;
        }
    }
}
