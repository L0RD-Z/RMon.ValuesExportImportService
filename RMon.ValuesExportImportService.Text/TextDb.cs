using RMon.Globalization.String;

namespace RMon.ValuesExportImportService.Text
{
    public class TextDb : IStringContainer
    {
        public static I18nString<string, string> SelectedNoOneLogicDeviceError = new(nameof(SelectedNoOneLogicDeviceError),
            "В базе данных не найдено логических устройств со свойством \"{propertyCode}\" равным {propertyValue}.", "propertyCode", "propertyValue");
        public static I18nString<int, string, string> SelectedManyLogicDeviceError = new(nameof(SelectedManyLogicDeviceError),
            "В базе данных найдено {count} логических устройств со свойством \"{propertyCode}\" равным {propertyValue}.", "count", "propertyCode", "propertyValue");
        public static I18nString<string> FindLogicDeviceNoOne = new(nameof(FindLogicDeviceNoOne), "В базе данных не найдено ни одного оборудования. Свойства: {properties}.",
            "properties");
        public static I18nString<string> FindTagNoOne = new(nameof(FindTagNoOne), "В базе данных не найдено ни одного тега. Свойства: {properties}.",
            "properties");
        public static I18nString<long, string> FindTagForLogicDeviceNoOne = new(nameof(FindTagForLogicDeviceNoOne), "В базе данных не найдено ни одного тега для оборудования (id = {idLogicDevice}). Свойства: {properties}.",
            "idLogicDevice", "properties");
    }
}
