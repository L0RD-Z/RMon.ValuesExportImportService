using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EsbPublisher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private MainLogic _logic;

        public MainLogic Logic
        {
            get => _logic;
            set
            {
                if (_logic != value)
                {
                    _logic = value;
                    OnPropertyChanged(nameof(Logic));
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Logic = new MainLogic();
            Task.Run(async () =>
            {
                await Logic.LoadDataAsync().ConfigureAwait(false);
            });
        }



        #region Комманды

        #region Экспорт

        private void Export_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting")]
        private async void Export_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            await Logic.SendExportTaskAsync().ConfigureAwait(true);
        }

        #endregion

        #region Отмена экспорта

        private void CancelExport_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = Logic?.ExportTaskAbortEnabled ?? false;
        }

        [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting")]
        private async void CancelExport_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            await Logic.SendExportTaskAbortAsync().ConfigureAwait(true);
        }

        #endregion


        #region Парсинг

        private void Parse_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private async void Parse_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            await Logic.SendParseTaskAsync().ConfigureAwait(true);
        }

        #endregion

        #region Отмена парсинга

        private void CancelParse_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private async void CancelParse_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            await Logic.SendParseTaskAbortAsync().ConfigureAwait(true);
        }

        #endregion


        #region Импорт

        private void Import_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting")]
        private async void Import_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            await Logic.SendImportTaskAsync().ConfigureAwait(true);
        }

        #endregion

        #region Отмена импорта

        private void CancelImport_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = Logic?.ImportTaskAbortEnabled ?? false;
        }

        [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting")]
        private async void CancelImport_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            await Logic.SendImportTaskAbortAsync().ConfigureAwait(true);
        }

        #endregion

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
