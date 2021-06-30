using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EsbPublisher.Annotations;

namespace EsbPublisher.Model
{
    public class Point : INotifyPropertyChanged
    {
        private string _propertyCode;
        private ObservableCollection<ChannelMap> _channels;
        private ChannelMap _selectedChannel;

        /// <summary>
        /// Код свойства оборудования
        /// </summary>
        public string PropertyCode
        {
            get => _propertyCode;
            set
            {
                if (_propertyCode != value)
                {
                    _propertyCode = value;
                    OnPropertyChanged(nameof(PropertyCode));
                }
            }
        }

        /// <summary>
        /// Список каналов
        /// </summary>
        public ObservableCollection<ChannelMap> Channels
        {
            get => _channels;
            set
            {
                if (_channels != value)
                {
                    _channels = value;
                    OnPropertyChanged(nameof(Channels));
                }
            }
        }

        /// <summary>
        /// Выбранный канал
        /// </summary>
        public ChannelMap SelectedChannel
        {
            get => _selectedChannel;
            set
            {
                if (_selectedChannel != value)
                {
                    _selectedChannel = value;
                    OnPropertyChanged(nameof(SelectedChannel));
                }
            }
        }


        public Point()
        {
            Channels = new ObservableCollection<ChannelMap>();
        }

        public Point(string propertyCode)
            :this()
        {
            PropertyCode = propertyCode;
        }

        /// <summary>
        /// Добавляет новый канал в список каналов
        /// </summary>
        public void AddChannel()
        {
            Channels.Add(new ChannelMap());
        }

        /// <summary>
        /// Удаляет <see cref="SelectedChannel"/> из списка каналов
        /// </summary>
        public void RemoveChannel()
        {
            if (SelectedChannel != null)
                Channels.Remove(SelectedChannel);
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
