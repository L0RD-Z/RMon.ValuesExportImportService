using System;

namespace RMon.ValuesExportImportService.Exceptions
{
    /// <summary>
    /// Класс исключений, созданных на уровне <see cref="ValuesExportImportService"/>
    /// </summary>
    public class HandledException : Exception
    {
        public HandledException() : base()
        {
        }

        public HandledException(string message) : base(message)
        {
        }

        public HandledException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
