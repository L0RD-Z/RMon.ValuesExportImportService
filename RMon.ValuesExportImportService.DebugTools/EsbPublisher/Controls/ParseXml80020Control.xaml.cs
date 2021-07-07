using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using EsbPublisher.Annotations;
using EsbPublisher.Processing.Parse;

namespace EsbPublisher.Controls
{
    /// <summary>
    /// Логика взаимодействия для ParseXml80020Control.xaml
    /// </summary>
    public partial class ParseXml80020Control : UserControl, INotifyPropertyChanged
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


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
