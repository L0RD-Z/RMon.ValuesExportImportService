using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using EsbPublisher.Annotations;
using EsbPublisher.Model;

namespace EsbPublisher.Processing.Parse
{

    public class ParseXml80020Logic : INotifyPropertyChanged
    {
        private Point _measuringPoint;
        private Point _deliveryPoint;

        /// <summary>
        /// Точка измерения
        /// </summary>
        public Point MeasuringPoint
        {
            get => _measuringPoint;
            set
            {
                if (_measuringPoint != value)
                {
                    _measuringPoint = value;
                    OnPropertyChanged(nameof(MeasuringPoint));
                }
            }
        }

        /// <summary>
        /// Точка поставки
        /// </summary>
        public Point DeliveryPoint
        {
            get => _deliveryPoint;
            set
            {
                if (_deliveryPoint != value)
                {
                    _deliveryPoint = value;
                    OnPropertyChanged(nameof(DeliveryPoint));
                }
            }
        }


        public ParseXml80020Logic()
        {
            MeasuringPoint = new Point();
            DeliveryPoint = new Point();
        }

        public void InitializeProperties()
        {
            MeasuringPoint = new Point("AgrNo")
            {
                Channels = new ObservableCollection<ChannelMap>
                {
                    new("01", "dHHA+")
                }
            };
        }


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
