using RMon.Globalization.String;

namespace RMon.ValuesExportImportService.Text
{
    public class TextDb : IStringContainer
    {
        public static I18nString<string, string> SelectedNoOneLogicDeviceError = new(nameof(SelectedNoOneLogicDeviceError),
            "В базе данных не найдено логических устройств со свойством \"{propertyCode}\" равным {propertyValue}.", "propertyCode", "propertyValue");
        public static I18nString<int, string, string> SelectedManyLogicDeviceError = new(nameof(SelectedManyLogicDeviceError),
            "В базе данных найдено {count} логических устройств со свойством \"{propertyCode}\" равным {propertyValue}.", "count", "propertyCode", "propertyValue");
    }
}
