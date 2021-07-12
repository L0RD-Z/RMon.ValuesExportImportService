using System;
using RMon.Globalization.String;

namespace RMon.ValuesExportImportService.Text
{
    public class TextParse : IStringContainer
    {
        public static I18nString LoadingFiles = new(nameof(LoadingFiles), "Загрузка файлов");
        public static I18nString<string, string> ReadingFile = new(nameof(ReadingFile), "Чтение файла формата \"{fileType}\": {fileName}", "fileName", "fileType");
        public static I18nString<string> ReadFileError = new(nameof(ReadFileError), "Не удалось выполнить чтение файла \"{fileName}\".", "fileName");
        
        public static I18nString<string> AnalyzeInfoFromFile = new(nameof(AnalyzeInfoFromFile), 
            "Анализ информации из файла \"{fileName}\"", "fileName");
        public static I18nString<string, string> AnalyzeInfoFromExcelFile = new(nameof(AnalyzeInfoFromExcelFile), 
            "Анализ информации из файла \"{fileName}\", страница \"{sheetName}\"", "fileName", "sheetName");
        public static I18nString<string, string> AnalyzeArea = new(nameof(AnalyzeArea), 
            "Анализ организации \"{name}\" (ИНН {inn})", "name", "inn");
        public static I18nString AnalyzeMeasuringPoints = new(nameof(AnalyzeMeasuringPoints), "Анализ точек измерения");
        public static I18nString AnalyzeDeliveryPoints = new(nameof(AnalyzeDeliveryPoints), "Анализ точек поставки");
        public static I18nString<string, string> AnalyzePoint = new(nameof(AnalyzeDeliveryPoints), 
            "Анализ точеки \"{name}\" (код: {code})", "name", "code");
        public static I18nString LoadingCurrentValues = new(nameof(LoadingCurrentValues), "Загрузка текущих значений из БД");
        public static I18nString UseTransformationRatio = new(nameof(UseTransformationRatio), "Применение коэффициентов трансформации");
        public static I18nString<long, DateTime, DateTime> LoadingCurrentValuesForTag = new(nameof(LoadingCurrentValuesForTag),
            "Загрузка текущих значений из БД для тега (id = {idTag}) за период [{dateStart}; {dateEnd}]", "idTag", "dateStart", "dateEnd");

        public static I18nString NoFilesError = new(nameof(NoFilesError), "Отсутствуют ссылки на файлы с данными для импорта.");
        public static I18nString NoUserIdError = new(nameof(NoUserIdError), "Не указан идентификатор пользователя.");
        public static I18nString<string, string, string> NotFoundSectionWarning = new(nameof(NotFoundSectionWarning),
            "В файле \"{fileName}\" у узла \"{areaName}\" отсутствует секция \"{sectionName}\".", "fileName", "areaName", "sectionName");

        public static I18nString<string, string, string> SelectedNoOneChannelsError = new(nameof(SelectedNoOneChannelsError),
            "У точки \"{pointName}\" (код: {pointCode}) не найдено каналов с кодом \"{channelCode}\".", "pointName", "pointCode", "channelCode");
        public static I18nString<string, string, string> SelectedManyChannelsError = new(nameof(SelectedNoOneChannelsError),
            "У точки \"{pointName}\" (код: {pointCode}) найдено более одного канала с кодом \"{channelCode}\".", "pointName", "pointCode", "channelCode");

        public static I18nString<long, string> SelectedNoOneTagsError = new(nameof(SelectedNoOneTagsError),
            "У оборудования (id = logicDeviceId) не найдено тегов с кодом \"{tagCode}\".", "logicDeviceId", "tagCode");

        public static I18nString<DateTime, DateTime> UndefinedTimestampError = new(nameof(UndefinedTimestampError),
            "Не удалось определить таймштамп у значения с {DateTimeStart} по {DateTimeEnd}", "DateTimeStart", "DateTimeEnd");

        public static I18nString<long, DateTime, DateTime> MissingValueWarning = new(nameof(MissingValueWarning),
            "Значение тега (id = {tagId}) с {DateTimeStart} по {DateTimeEnd} является не полным и будет пропущено.", "tagId", "DateTimeStart", "DateTimeEnd");

        public static I18nString<string, string> MissingTimestampTypeError = new(nameof(MissingTimestampTypeError),
            "Тег \"{tagCode}\" имеет неподдерживаемый тип таймстампа. Импорт значений в формате 80020 возможен только для тегов с таймстампами {supportedTimestampTypes}", "tagCode", "supportedTimestampTypes");

        public static I18nString<string, string> SheetParseError = new(nameof(SheetParseError),
            "Файл \"{fileName}\" лист \"{sheetName}\": произошло исключение.", "fileName", "sheetName");
        public static I18nString<string, string, int> RowParseError = new(nameof(RowParseError),
            "Файл \"{fileName}\" лист \"{sheetName}\" строка \"{rowNumber}\": произошло исключение.", "fileName", "sheetName", "rowNumber");
        public static I18nString<string> MissingSectionError = new(nameof(MissingSectionError),
            "Отсутствует секция \"{sectionName}\".", "sectionName");
        public static I18nString<string, string> MissingSectionsError = new(nameof(MissingSectionsError),
            "Отсутствуют секции \"{logicDeviceSection}\" и \"{tagSection}\".", "logicDeviceSection", "tagSection");
        public static I18nString<string> MissingPropertyError = new(nameof(MissingSectionsError),
            "Отсутствует свойство \"{propertyName}\".", "propertyName");
        public static I18nString<string> FailedConvertToDateTimeError = new(nameof(FailedConvertToDateTimeError),
            "Не удалось преобразовать значение {value} в дату и время.", "value");
        public static I18nString<string> FailedConvertToDoubleError = new(nameof(FailedConvertToDoubleError),
            "Не удалось преобразовать значение {value} в число с плавающей точкой.", "value");
        public static I18nString<string> FailedConvertToLongError= new(nameof(FailedConvertToLongError),
            "Не удалось преобразовать значение {value} в целое число.", "value");


        public static I18nString MissingLogicDevicePropertyCodeError = new(nameof(MissingLogicDevicePropertyCodeError), "В параметрах задания отсутствует код свойства оборудования.");
        public static I18nString MissingLogicDevicePropertyCellAddressError = new(nameof(MissingLogicDevicePropertyCellAddressError), "В параметрах задания отсутствует адрес ячейки со значением свойства оборудования.");
        public static I18nString MissingLogicDevicePropertyRowNumberError = new(nameof(MissingLogicDevicePropertyRowNumberError), "В параметрах задания отсутствует номер строки со значением свойства оборудования.");
        public static I18nString MissingTagCodeError = new(nameof(MissingTagCodeError), "В параметрах задания отсутствует код тега.");
        public static I18nString MissingFirstValueCellAddressError = new(nameof(MissingFirstValueCellAddressError), "В параметрах задания отсутствует адрес начальной ячейки матрицы.");
        public static I18nString MissingDateColumnNumberError = new(nameof(MissingDateColumnNumberError), "В параметрах задания отсутствует номер столбца с датами.");
        public static I18nString MissingDateRowNumberError = new(nameof(MissingDateRowNumberError), "В параметрах задания отсутствует номер строки с датами.");
        public static I18nString MissingTimeColumnNumberError = new(nameof(MissingTimeColumnNumberError), "В параметрах задания отсутствует номер столбца с часами.");
        public static I18nString MissingTimeRowNumberError = new(nameof(MissingTimeRowNumberError), "В параметрах задания отсутствует номер строки с часами.");
        public static I18nString IncorrectDateRowNumberError = new(nameof(IncorrectDateRowNumberError), "В параметрах задания номер строки с датами задан некорректно.");
        public static I18nString IncorrectTimeRowNumberError = new(nameof(IncorrectTimeRowNumberError), "В параметрах задания номер строки с часами задан некорректно.");
        public static I18nString IncorrectLogicDevicePropertyRowNumberError = new(nameof(IncorrectLogicDevicePropertyRowNumberError), "В параметрах задания номер строки с со значениями свойств оборудования задан некорректно.");

        public static I18nString<string, char> InvalidCharactersError = new(nameof(InvalidCharactersError), 
            "Номер столбца \"{number}\" содержит недопустимый символ \"{ch}\".", "number", "ch");
        public static I18nString<string> InvalidCellAddressError = new(nameof(InvalidCellAddressError),
            "Адрес ячейки \"{cellAddress}\" задан некорректно.", "cellAddress");
    }
}
