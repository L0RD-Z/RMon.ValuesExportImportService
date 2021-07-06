using System;
using RMon.Globalization.String;

namespace RMon.ValuesExportImportService.Text
{
    public class TextExport : IStringContainer
    {
        public static I18nString LoadingData = new(nameof(LoadingData), "Загрузка из базы данных");
        public static I18nString BuildingExcel = new(nameof(BuildingExcel), "Формирование Excel файла");
        public static I18nString StoringFile = new(nameof(StoringFile), "Отправка файла в файловое хранилище");
        
        public static I18nString<DateTime> ExportFileName = new (nameof(ExportFileName), "Выгрузка значений {dateTime:yyyyMMdd HHmmss}.xlsx", "dateTime");
        
        public static I18nString NoUserIdError = new(nameof(NoUserIdError), "Не указан идентификатор пользователя.");
        public static I18nString NoLogicDevicesError = new(nameof(NoLogicDevicesError), "Список оборудования пуст.");
        public static I18nString NoTagCodesError = new(nameof(NoTagCodesError), "Список кодов тегов пуст.");
    }
}
