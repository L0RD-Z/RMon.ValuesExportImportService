using System;
using System.Globalization;
using System.Windows.Data;
using EsbPublisher.Model;

namespace EsbPublisher.Converters
{
    [ValueConversion(typeof(Operations), typeof(string))]
    public class OperationsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Operations)value switch
            {
                Operations.Export => "Экспорт",
                Operations.Parse => "Парсинг",
                Operations.Import => "Импорт",
                _ => "Не распознано"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
