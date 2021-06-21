using System;
using RMon.Globalization.String;
using RMon.ValuesExportImportService.Exceptions;

namespace RMon.ValuesExportImportService.Excel.Common
{
    class ExcelException : UserException
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
