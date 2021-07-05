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


        public static I18nString<int> RowUnexpectedError = new (nameof(RowUnexpectedError), 
            "Во время заполнения строки {rowIndex} произошло исключение.", "rowIndex");
        public static I18nString<I18nString> SheetUnexpectedError = new (nameof(SheetUnexpectedError), 
            "Лист \"{EntityName}\": во время выполнения процесса формирования произошло исключение.", "EntityName");
        
        public static I18nString<string> SheetParseUnexpectedError = new(nameof(SheetParseUnexpectedError), 
            "Лист \"{sheetName}\": не удалось распарсить.", "sheetName");
        public static I18nString<int> RowParseUnexpectedError = new(nameof(RowParseUnexpectedError), 
            "Не удалось распарсить строку № {rowIndex}.", "rowIndex");
        public static I18nString<int> ColParseUnexpectedError = new(nameof(ColParseUnexpectedError),
            "Не удалось распарсить столбец № {colIndex}.", "colIndex");

        public static I18nString<string> ParseHierarchyNameError = new (nameof(ParseHierarchyNameError), 
            "Не удалось получить название иерархии из строки \"{string}\".", "string");

        public static I18nString<string> IncorrectDateFormatError = new(nameof(IncorrectDateFormatError), 
            "Не удалось распарсить дату \"{date}\".", "date");
        public static I18nString<string> IncorrectTimeFormatError = new(nameof(IncorrectTimeFormatError),
            "Не удалось распарсить время \"{time}\".", "time");
        public static I18nString<string> IncorrectHoursFormatError = new(nameof(IncorrectHoursFormatError), 
            "Не удалось распарсить строку с часами \"{hours}\".", "hours");
        public static I18nString<string> IncorrectValueFormatError = new(nameof(IncorrectValueFormatError), 
            "Не удалось распарсить строку со значением \"{strValue}\".", "strValue");

        public static I18nString FailedParseLogicDevicePropertyValueError = new(nameof(FailedParseLogicDevicePropertyValueError), "Не удалось получить значение свойства оборудования.");

    }
}    
