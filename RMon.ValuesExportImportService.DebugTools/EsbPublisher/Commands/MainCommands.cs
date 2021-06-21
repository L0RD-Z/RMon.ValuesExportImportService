using System.Windows.Input;

namespace EsbPublisher.Commands
{
    public static class MainCommands
    {
        public static RoutedUICommand Export { get; }
        public static RoutedUICommand CancelExport { get; }
        public static RoutedUICommand Import { get; }
        public static RoutedUICommand CancelImport { get; }


        static MainCommands()
        {
            Export = new RoutedUICommand("Экспорт", nameof(Export), typeof(MainCommands), null);
            CancelExport = new RoutedUICommand("Отмена", nameof(CancelExport), typeof(MainCommands), null);
            Import = new RoutedUICommand("Импорт", nameof(Import), typeof(MainCommands), null);
            CancelImport = new RoutedUICommand("Отмена", nameof(CancelImport), typeof(MainCommands), null);
        }
    }
}
