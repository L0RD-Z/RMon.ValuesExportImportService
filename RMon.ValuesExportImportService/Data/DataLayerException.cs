using System;
using RMon.Globalization.String;
using RMon.ValuesExportImportService.Exceptions;

namespace RMon.ValuesExportImportService.Data
{
    public class DataLayerException : UserException
    {
        public DataLayerException(I18nString message) : base(message)
        {
        }

        public DataLayerException(I18nString message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
