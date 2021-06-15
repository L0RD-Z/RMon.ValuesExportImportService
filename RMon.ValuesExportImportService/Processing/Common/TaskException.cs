using System;
using RMon.Globalization.String;
using RMon.ValuesExportImportService.Exceptions;

namespace RMon.ValuesExportImportService.Processing.Common
{
    class TaskException : UserException
    {
        public TaskException(I18nString message) : base(message)
        {
        }

        public TaskException(I18nString message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
