using System;
using RMon.Globalization.String;
using RMon.ValuesExportImportService.Exceptions;

namespace RMon.ValuesExportImportService.Files
{
    public class FileStorageException : UserException
    {
        public FileStorageException(I18nString message)
            : base(message)
        {
        }

        public FileStorageException(I18nString message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
