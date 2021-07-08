using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Input;
using EsbPublisher.Processing;

namespace EsbPublisher.Controls.OperationsControl
{
    /// <summary>
    /// Логика взаимодействия для ExportControl.xaml
    /// </summary>
    public partial class ExportControl : OperationControlBase
    {
        private ExportLogic _logic;

        public ExportLogic Logic
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


        public ExportControl(ExportLogic logic)
        {
            InitializeComponent();
            Logic = logic;

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
            await Logic.SendTaskAsync().ConfigureAwait(true);
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
            await Logic.SendTaskAbortAsync().ConfigureAwait(true);
        }

        #endregion

        #endregion
    }
}
