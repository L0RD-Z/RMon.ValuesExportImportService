using RMon.Globalization.String;

namespace RMon.ValuesExportImportService.Text
{
    public class TextExcel : IStringContainer
    {
        public static I18nString EntityProperties = new(nameof(EntityProperties), "Свойства сущностей");

        public static I18nString Values = new(nameof(Values), "Значения");
        public static I18nString Tag = new(nameof(Tag), "Тег");
        public static I18nString Id = new(nameof(Id), "Идентификатор");
        public static I18nString Code = new(nameof(Code), "Код");
        public static I18nString Name = new(nameof(Name), "Название");
        public static I18nString Timestamp = new(nameof(Timestamp), "Дата и время");
        public static I18nString Value = new(nameof(Value), "Значение");

        public static I18nString Tags = new(nameof(Tag), "Теги");
        public static I18nString Results = new(nameof(Tag), "Результаты");


        public static I18nString<int> RowUnexpectedError = new (nameof(RowUnexpectedError), "Во время заполнения строки {rowIndex} произошло исключение.",
            "rowIndex");
        public static I18nString<I18nString> SheetUnexpectedError = new (nameof(SheetUnexpectedError), "Лист \"{EntityName}\": во время выполнения процесса формирования произошло исключение.",
            "EntityName");
        public static I18nString<I18nString, int> ParseUnexpectedError = new (nameof(ParseUnexpectedError), "Лист \"{EntityName}\": не удалось распарсить строку №{rowIndex}.",
            "EntityName", "rowIndex");

        public static I18nString<string> ParseHierarchyNameError = new (nameof(ParseHierarchyNameError), "Не удалось получить название иерархии из строки \"{string}\"",
            "string");
        
    }
}    
