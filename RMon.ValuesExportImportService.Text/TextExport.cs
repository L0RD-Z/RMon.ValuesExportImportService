using System;
using RMon.Globalization.String;

namespace RMon.ValuesExportImportService.Text
{
    public class TextExport : IStringContainer
    {
        public static I18nString Start = new(nameof(Start), "Начало выполнения экспорта");
        public static I18nString ValidateParameters = new(nameof(ValidateParameters), "Проверка корректности параметров");
        public static I18nString LoadingData = new(nameof(LoadingData), "Загрузка из базы данных");
        public static I18nString BuildingExcel = new(nameof(BuildingExcel), "Формирование Excel файла");
        public static I18nString StoringFile = new(nameof(StoringFile), "Отправка файла в файловое хранилище");
        public static I18nString FinishSuccess = new(nameof(FinishSuccess), "Задание завершено успешно");
        public static I18nString FinishAborted = new(nameof(FinishAborted), "Задание отменено");
        public static I18nString<I18nString> FinishFailed = new (nameof(FinishFailed), "Ошибка при выполнении задания: {error}", "error");

        public static I18nString<DateTime> ExportFileName = new (nameof(ExportFileName), "Выгрузка значений {dateTime:yyyyMMdd HHmmss}.xlsx", "dateTime");
        
        public static I18nString NoUserIdError = new(nameof(NoUserIdError), "Не указан идентификатор пользователя.");
        public static I18nString NoLogicDevicesError = new(nameof(NoLogicDevicesError), "Список оборудования пуст.");
        public static I18nString NoTagCodesError = new(nameof(NoTagCodesError), "Список кодов тегов пуст.");
    }
}
