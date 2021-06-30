using System;
using RMon.Globalization;
using RMon.Globalization.String;


namespace RMon.ValuesExportImportService.Processing.Parse.Format80020
{
    public class Format800200Exception : UserFormattedException
    {
        public Format800200Exception(I18nString message) : base(message)
        {
        }

        public Format800200Exception(I18nString message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
