using System;
using RMon.Globalization;
using RMon.Globalization.String;


namespace RMon.ValuesExportImportService.Processing.Common
{
    class TaskException : UserFormattedException
    {
        public TaskException(I18nString message) : base(message)
        {
        }

        public TaskException(I18nString message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
