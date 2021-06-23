using System.Windows.Input;

namespace EsbPublisher.Commands
{
    public static class PointsCommands
    {
        public static RoutedUICommand MeasuringAddPoint { get; }
        public static RoutedUICommand MeasuringRemovePoint { get; }
        public static RoutedUICommand DeliveryAddPoint { get; }
        public static RoutedUICommand DeliveryRemovePoint { get; }


        static PointsCommands()
        {
            MeasuringAddPoint = new RoutedUICommand("Добавить точку измерения", nameof(MeasuringAddPoint), typeof(MainCommands), null);
            MeasuringRemovePoint = new RoutedUICommand("Удалить точку измерения", nameof(MeasuringRemovePoint), typeof(MainCommands), null);
            DeliveryAddPoint = new RoutedUICommand("Добавить точку поставки", nameof(DeliveryAddPoint), typeof(MainCommands), null);
            DeliveryRemovePoint = new RoutedUICommand("Удалить точку поставки", nameof(DeliveryRemovePoint), typeof(MainCommands), null);
        }
    }
}
