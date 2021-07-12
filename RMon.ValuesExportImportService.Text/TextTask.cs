using RMon.Globalization.String;

namespace RMon.ValuesExportImportService.Text
{
    public class TextTask : IStringContainer
    {
        public static I18nString Start = new(nameof(Start), "Начало выполнения задания");
        public static I18nString ValidateParameters = new(nameof(ValidateParameters), "Проверка корректности параметров задания");
        public static I18nString FinishSuccess = new(nameof(FinishSuccess), "Задание завершено успешно");
        public static I18nString<I18nString> FinishFailed = new(nameof(FinishFailed), "Задание завершено с ошибкой: {error}", "error");
        public static I18nString FinishAborted = new(nameof(FinishAborted), "Задание отменено");
    }
}
