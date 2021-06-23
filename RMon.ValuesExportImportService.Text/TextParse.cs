using System;
using RMon.Globalization.String;

namespace RMon.ValuesExportImportService.Text
{
    public class TextParse : IStringContainer
    {
        public static I18nString Start = new(nameof(Start), "Начало выполнения парсинга");
        public static I18nString ValidateParameters = new(nameof(ValidateParameters), "Проверка корректности параметров");
        public static I18nString LoadingFiles = new(nameof(LoadingFiles), "Загрузка файлов");
        public static I18nString<string, string> ReadingFile = new(nameof(ReadingFile), "Чтение файла формата \"{fileType}\": {fileName}", "fileName", "fileType");
        public static I18nString FinishSuccess = new(nameof(FinishSuccess), "Задание завершено успешно");
        public static I18nString FinishAborted = new(nameof(FinishAborted), "Задание отменено");
        public static I18nString<I18nString> FinishFailed = new(nameof(FinishFailed), "Ошибка при выполнении задания: {error}", "error");
        public static I18nString<string> AnalyzeInfoFromFile = new(nameof(AnalyzeInfoFromFile), "Анализ информации из файла \"{fileName}\"", "fileName");
        public static I18nString<string, string> AnalyzeArea = new(nameof(AnalyzeArea), 
            "Анализ организации \"{name}\" (ИНН {inn})", "name", "inn");
        public static I18nString AnalyzeMeasuringPoints = new(nameof(AnalyzeMeasuringPoints), "Анализ точек измерения");
        public static I18nString AnalyzeDeliveryPoints = new(nameof(AnalyzeDeliveryPoints), "Анализ точек поставки");
        public static I18nString<string, string> AnalyzePoint = new(nameof(AnalyzeDeliveryPoints), 
            "Анализ точеки \"{name}\" (код: {code})", "name", "code");


        public static I18nString NoFilesError = new(nameof(NoFilesError), "Отсутствуют ссылки на файлы с данными для импорта.");
        public static I18nString NoUserIdError = new(nameof(NoUserIdError), "Не указан идентификатор пользователя.");
        public static I18nString<string> NotFoundParameterError = new(nameof(NotFoundParameterError), "Отсутствует параметр \"{parameterName}\".", "parameterName");

        public static I18nString<string, string, string> NotFoundSectionWarning = new(nameof(NotFoundSectionWarning),
            "В файле \"{fileName}\" у узла \"{areaName}\" отсутствует секция \"{sectionName}\".", "fileName", "areaName", "sectionName");

        public static I18nString<string, string> SelectedNoOneChannelsError = new(nameof(SelectedNoOneChannelsError),
            "В настройках задания \"{settingsName}\" не найдено каналов с кодом \"{channelCode}\".", "settingsName", "channelCode");
        public static I18nString<string, int, string> SelectedManyChannelsError = new(nameof(SelectedNoOneChannelsError),
            "В настройках задания \"{settingsName}\" найдено {channelsCount} каналов с кодом \"{channelCode}\".", "settingsName", "channelsCount", "channelCode");

        public static I18nString<long, string> SelectedNoOneTagsError = new(nameof(SelectedNoOneTagsError),
            "У оборудования (id = logicDeviceId) не найдено тегов с кодом \"{tagCode}\".", "logicDeviceId", "tagCode");
        public static I18nString<long, int, string> SelectedManyTagsError = new(nameof(SelectedManyTagsError),
            "У оборудования (id = logicDeviceId) найдено {tagCount} тегов с кодом \"{tagCode}\".", "logicDeviceId", "tagCount", "tagCode");

        public static I18nString<DateTime, DateTime> UndefinedTimestampError = new(nameof(UndefinedTimestampError),
            "Не удалось определить таймштамп у значения с {DateTimeStart} по {DateTimeEnd}", "DateTimeStart", "DateTimeEnd");
        public static I18nString<string> FailedToConvertToDateTimeError = new(nameof(FailedToConvertToDateTimeError),
            "Не удалось преобразовать значение {value} в дату и время.", "value");

        public static I18nString<long, DateTime, DateTime> MissingValueWarning = new(nameof(MissingValueWarning),
            "Значение тега (id = {tagId}) с {DateTimeStart} по {DateTimeEnd} является не полным и будет пропущено.", "tagId", "DateTimeStart", "DateTimeEnd");

        public static I18nString<string, string> MissingTimestampTypeError = new(nameof(MissingTimestampTypeError),
            "Тег \"{tagCode}\" имеет неподдерживаемый тип таймстампа. Импорт значений в формате 80020 возможен только для тегов с таймстампами {supportedTimestampTypes}", "tagCode", "supportedTimestampTypes");
    }
}
