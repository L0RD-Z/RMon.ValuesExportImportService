using System.Windows.Input;
using EsbPublisher.Processing.Parse;

namespace EsbPublisher.Controls.ParseControls
{
    /// <summary>
    /// Логика взаимодействия для ParseXml80020Control.xaml
    /// </summary>
    public partial class ParseXml80020Control : ParseControlBase
    {
        private ParseXml80020Logic _logic;

        public ParseXml80020Logic Logic
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

        public ParseXml80020Control(ParseXml80020Logic logic)
        {
            InitializeComponent();
            Logic = logic;
        }


        #region Команды

        #region Точки измерения

        #region Добавить канал

        private void MeasuringAddPoint_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void MeasuringAddPoint_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Logic.MeasuringPoint.AddChannel();
        }

        #endregion


        #region Удалить канал

        private void MeasuringRemovePoint_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Logic?.MeasuringPoint?.SelectedChannel != null;
        }

        private void MeasuringRemovePoint_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Logic.MeasuringPoint.RemoveChannel();
        }

        #endregion

        #endregion

        #region Точки поставки

        #region Добавить канал

        private void DeliveryAddPoint_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void DeliveryAddPoint_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Logic.DeliveryPoint.AddChannel();
        }

        #endregion


        #region Удалить канал

        private void DeliveryRemovePoint_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Logic?.DeliveryPoint?.SelectedChannel != null;
        }

        private void DeliveryRemovePoint_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Logic.DeliveryPoint.RemoveChannel();
        }

        #endregion

        #endregion

        #endregion
    }
}
