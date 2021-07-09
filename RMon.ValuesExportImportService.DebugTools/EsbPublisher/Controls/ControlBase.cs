using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using EsbPublisher.Annotations;

namespace EsbPublisher.Controls
{
    public class ControlBase : UserControl, INotifyPropertyChanged
    {

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
