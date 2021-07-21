using RMon.Globalization.String;

namespace RMon.ValuesExportImportService.Text
{
    public class Text80020 : IStringContainer
    {
        public static I18nString IncorrectFileFormatError = new(nameof(IncorrectFileFormatError), "Некорректный формат файла.");
        public static I18nString Not80020Error = new(nameof(Not80020Error), "Значение атрибута \"class\" не равно \"80020\"");
        public static I18nString UnsupportedVersionError = new(nameof(UnsupportedVersionError), "Неподдерживаемая версия файла");
    }
}
