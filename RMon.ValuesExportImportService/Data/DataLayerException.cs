using System;
using RMon.Globalization;
using RMon.Globalization.String;

namespace RMon.ValuesExportImportService.Data
{
    public class DataLayerException : UserFormattedException
    {
        public DataLayerException(I18nString message) : base(message)
        {
        }

        public DataLayerException(I18nString message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
