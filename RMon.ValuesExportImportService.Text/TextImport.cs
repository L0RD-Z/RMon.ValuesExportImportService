using RMon.Globalization.String;

namespace RMon.ValuesExportImportService.Text
{
    public class TextImport : IStringContainer
    {
        public static I18nString DeleteCurrentValues = new(nameof(DeleteCurrentValues), "Удаление \"текущих значений\" из БД");

        public static I18nString<string, long> TagImportSuccess = new(nameof(TagImportSuccess), 
        "Значения тега \"{tagCode}\" (id = {tagId}) импортированны успешно", "tagCode", "tagId");

        public static I18nString<long> TagNotFoundError = new(nameof(TagNotFoundError), 
            "Не удалось найти тег (id = {tagId}).", "tagId");
        public static I18nString<long> TagNotImportError = new(nameof(TagNotImportError),
            "Не удалось импортировать тег (id = {tagId}).", "tagId");
    }
}
