using System;
using RMon.Globalization;
using RMon.Globalization.String;


namespace RMon.ValuesExportImportService.Excel.Common
{
    class ExcelException : UserFormattedException
    {
        public ExcelException(I18nString message)
            : base(message)
        {
        }

        public ExcelException(I18nString message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
