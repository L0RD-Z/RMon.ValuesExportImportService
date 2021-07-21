using System;
using RMon.Globalization;
using RMon.Globalization.String;


namespace RMon.ValuesExportImportService.Processing.Parse.Format80020
{
    public class Format80020Exception : UserFormattedException
    {
        public Format80020Exception(I18nString message) : base(message)
        {
        }

        public Format80020Exception(I18nString message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
