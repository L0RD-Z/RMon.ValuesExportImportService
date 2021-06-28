using System;
using RMon.Globalization;
using RMon.Globalization.String;

namespace RMon.ValuesExportImportService.Processing.Parse.Common
{
    class ParseException : UserFormattedException
    {
        public ParseException(I18nString formattedMessage) : base(formattedMessage)
        {
        }

        public ParseException(I18nString formattedMessage, Exception innerException) : base(formattedMessage, innerException)
        {
        }
    }
}
