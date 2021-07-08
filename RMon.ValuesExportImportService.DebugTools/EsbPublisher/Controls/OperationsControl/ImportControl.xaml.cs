using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using EsbPublisher.Processing;

namespace EsbPublisher.Controls.OperationsControl
{
    /// <summary>
    /// Логика взаимодействия для ImportControl.xaml
    /// </summary>
    public partial class ImportControl : OperationControlBase
    {
        private ImportLogic _logic;

        public ImportLogic Logic
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
        
        
        public ImportControl(ImportLogic logic)
        {
            InitializeComponent();
            Logic = logic;
        }


        #region Комманды

        #region Импорт

        private void Import_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting")]
        private async void Import_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                await Logic.SendTaskAsync().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current.MainWindow, ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            await Logic.SendTaskAbortAsync().ConfigureAwait(true);
        }

        #endregion

        #endregion
    }
}
