using RMon.Globalization.String;

namespace RMon.ValuesExportImportService.Text
{
    public class TextDb : IStringContainer
    {
        public static I18nString<string, string> SelectedNoOneLogicDeviceError = new(nameof(SelectedNoOneLogicDeviceError),
            "В базе данных не найдено оборудование со свойством \"{propertyCode}\" равным {propertyValue}.", "propertyCode", "propertyValue");
        public static I18nString<string, string> SelectedManyLogicDeviceError = new(nameof(SelectedManyLogicDeviceError),
            "В базе данных найдено более одного оборудования со свойством \"{propertyCode}\" равным {propertyValue}.", "propertyCode", "propertyValue");
        
        public static I18nString<string> FindNoOneLogicDeviceError = new(nameof(FindNoOneLogicDeviceError),
            "В базе данных не найдено ни одного оборудования со свойствами: {properties}.", "properties");
        public static I18nString<string> FindManyLogicDevicesError = new(nameof(FindManyLogicDevicesError),
            "В базе данных найдено более одного оборудования со свойствами: {properties}.", "properties");
        
        public static I18nString<string> FindNoOneTagError = new(nameof(FindNoOneTagError), 
            "В базе данных не найдено ни одного тега со свойствами: {properties}.", "properties");
        public static I18nString<string, long> FindNoOneTagForLogicDevice = new(nameof(FindNoOneTagForLogicDevice),
            "В базе данных не найдено ни одного тега со свойствами: {properties} для оборудования (id = {idLogicDevice}).", "properties", "idLogicDevice");
    }
}
