using RMon.Globalization.String;

namespace RMon.ValuesExportImportService.Text
{
    public class TextFileStorage: IStringContainer
    {
        public static I18nString<string> ErrorCodeException = new (nameof(ErrorCodeException), "Код ошибки: {code}.", "code");

        public static I18nString<string, string> SendFileException = new(nameof(SendFileException),
            "Во время отправки файла \"{filePath}\" в файловое хранилище \"{options}\" возникло исключение.", "filePath", "options");
        public static I18nString<string, string, string> SendFileWithStatusException = new(nameof(SendFileWithStatusException),
            "Во время отправки файла \"{filePath}\" в файловое хранилище \"{options}\" возникло исключение. Статус: \"{status}\"", "filePath", "options", "status");

        public static I18nString<string, string> ReceiveFileException = new(nameof(ReceiveFileException),
            "Во время получения файла \"{filePath}\" из файлового хранилища \"{options}\" возникло исключение.", "filePath", "options");
        public static I18nString<string, string, string> ReceiveFileWithStatusException = new(nameof(ReceiveFileWithStatusException),
            "Во время получения файла \"{filePath}\" из файлового хранилища \"{options}\" возникло исключение. Статус: \"{status}\"", "filePath", "options", "status");

        public static I18nString<string, string> DeleteFileException = new(nameof(DeleteFileException),
            "Во время удаления файла \"{filePath}\" из файлового хранилища \"{options}\" возникло исключение.", "filePath", "options");
        public static I18nString<string, string, string> DeleteFileWithStatusException = new(nameof(DeleteFileWithStatusException),
            "Во время удаления файла \"{filePath}\" из файлового хранилища \"{options}\" возникло исключение. Статус: \"{status}\"", "filePath", "options", "status");

        public static I18nString<string, string> GetFileInfoException = new(nameof(GetFileInfoException),
            "Во время получения информации о файле \"{filePath}\" из файлового хранилища \"{options}\" возникло исключение.", "filePath", "options");
        public static I18nString<string, string, string> GetFileInfoWithStatusException = new(nameof(GetFileInfoWithStatusException),
            "Во время получения информации о файле \"{filePath}\" из файлового хранилища \"{options}\" возникло исключение. Статус: \"{status}\"", "filePath", "options", "status");
    }
}
