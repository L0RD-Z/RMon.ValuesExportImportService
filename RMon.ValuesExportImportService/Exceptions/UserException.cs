using System;
using RMon.Globalization.String;

namespace RMon.ValuesExportImportService.Exceptions
{
    /// <summary>
    /// Класс пользовательских исключений, созданных на уровне <see cref="CommissioningService"/>
    /// </summary>
    public class UserException : Exception
    {
        public I18nString String { get; }

        public UserException(I18nString message) : base(message.ToString())
        {
            String = message;
        }

        public UserException(I18nString message, Exception innerException) : base(message.ToString(), innerException)
        {
            String = message;
        }
    }
}
