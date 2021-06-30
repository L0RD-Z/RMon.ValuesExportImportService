using RMon.Globalization.String;

namespace RMon.ValuesExportImportService.Text
{
    public class TextDb : IStringContainer
    {
        public static I18nString<string, string> SelectedNoOneLogicDeviceError = new(nameof(SelectedNoOneLogicDeviceError),
            "В базе данных не найдено логических устройств со свойством \"{propertyCode}\" равным {propertyValue}.", "propertyCode", "propertyValue");
        public static I18nString<int, string, string> SelectedManyLogicDeviceError = new(nameof(SelectedManyLogicDeviceError),
            "В базе данных найдено {count} логических устройств со свойством \"{propertyCode}\" равным {propertyValue}.", "count", "propertyCode", "propertyValue");
        
        public static I18nString<string> FindLogicDeviceNoOneError = new(nameof(FindLogicDeviceNoOneError),
            "В базе данных не найдено ни одного оборудования со свойствами: {properties}.", "properties");
        public static I18nString<int, string> FindManyLogicDevicesError = new(nameof(FindManyLogicDevicesError),
            "В базе данных найдено {count} логических устройств со свойствами: {properties}.", "properties", "count");
        
        public static I18nString<string> FindTagNoOne = new(nameof(FindTagNoOne), "В базе данных не найдено ни одного тега. Свойства: {properties}.",
            "properties");
        public static I18nString<long, string> FindTagForLogicDeviceNoOne = new(nameof(FindTagForLogicDeviceNoOne), "В базе данных не найдено ни одного тега для оборудования (id = {idLogicDevice}). Свойства: {properties}.",
            "idLogicDevice", "properties");
    }
}
