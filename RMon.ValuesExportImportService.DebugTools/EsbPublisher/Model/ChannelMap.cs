using System.ComponentModel;
using System.Runtime.CompilerServices;
using EsbPublisher.Annotations;

namespace EsbPublisher.Model
{
    public class ChannelMap : INotifyPropertyChanged
    {
        private string _channelCode;
        private string _tagCode;

        /// <summary>
        /// Код канала
        /// </summary>
        public string ChannelCode
        {
            get => _channelCode;
            set
            {
                if (_channelCode != value)
                {
                    _channelCode = value;
                    OnPropertyChanged(nameof(ChannelCode));
                }
            }
        }
        /// <summary>
        /// Код тега
        /// </summary>
        public string TagCode
        {
            get => _tagCode;
            set
            {
                if (_tagCode != value)
                {
                    _tagCode = value;
                    OnPropertyChanged(nameof(TagCode));
                }
            }
        }


        public ChannelMap()
        {
            
        }


        public ChannelMap(string channelCode, string tagCode)
        {
            _channelCode = channelCode;
            _tagCode = tagCode;
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
