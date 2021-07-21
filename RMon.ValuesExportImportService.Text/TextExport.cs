using System;
using RMon.Globalization.String;

namespace RMon.ValuesExportImportService.Text
{
    public class TextExport : IStringContainer
    {
        public static I18nString LoadingData = new(nameof(LoadingData), "Загрузка данных из базы данных");
        public static I18nString BuildingExcel = new(nameof(BuildingExcel), "Формирование Excel - файла");
        public static I18nString StoringFile = new(nameof(StoringFile), "Отправка файла в файловое хранилище");
        
        public static I18nString<DateTime> ExportFileName = new (nameof(ExportFileName), "Выгрузка значений {dateTime:yyyyMMdd HHmmss}.xlsx", "dateTime");
        
        public static I18nString MissingUserIdError = new(nameof(MissingUserIdError), "В параметрах задания отсутствует идентификатор пользователя.");
        public static I18nString MissingLogicDevicesError = new(nameof(MissingLogicDevicesError), "В параметрах задания отсутствует оборудование.");
        public static I18nString MissingTagCodesError = new(nameof(MissingTagCodesError), "В параметрах задания отсутствуют типы тегов.");
    }
}
