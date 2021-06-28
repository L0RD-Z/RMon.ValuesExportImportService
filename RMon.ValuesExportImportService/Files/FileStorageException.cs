using System;
using RMon.Globalization;
using RMon.Globalization.String;


namespace RMon.ValuesExportImportService.Files
{
    public class FileStorageException : UserFormattedException
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
