using RMon.Globalization.String;

namespace RMon.ValuesExportImportService.Text
{
    public class Text80020 : IStringContainer
    {
        public static I18nString IncorrectFileFormat = new(nameof(IncorrectFileFormat), "Некорректный формат файла.");
        public static I18nString Not80020 = new(nameof(Not80020), "Значение атрибута \"class\" не равно \"800200\"");
        public static I18nString UnsupportedVersion = new(nameof(UnsupportedVersion), "Неподдерживаемая версия файла");
    }
}
