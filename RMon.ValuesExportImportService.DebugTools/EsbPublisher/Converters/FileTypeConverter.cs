using System;
using System.Globalization;
using System.Windows.Data;
using RMon.Values.ExportImport.Core;

namespace EsbPublisher.Converters
{
    [ValueConversion(typeof(ValuesParseFileFormatType), typeof(string))]
    public class ImportSettingsTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (ValuesParseFileFormatType)value switch
            {
                ValuesParseFileFormatType.Xml80020 => "Формат 80020",
                ValuesParseFileFormatType.Matrix24X31 => "Матрица 24x31",
                ValuesParseFileFormatType.Matrix31X24 => "Матрица 31x24",
                ValuesParseFileFormatType.Table => "Таблица",
                ValuesParseFileFormatType.Flexible => "Гибкий формат",
                _ => "Не распознано"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
