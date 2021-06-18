using System;
using RMon.Globalization.String;
using RMon.ValuesExportImportService.Exceptions;

namespace RMon.ValuesExportImportService.Processing.Parse.Format80020
{
    public class Format800200Exception : UserException
    {
        public Format800200Exception(I18nString message) : base(message)
        {
        }

        public Format800200Exception(I18nString message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
