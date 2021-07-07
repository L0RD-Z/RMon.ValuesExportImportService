using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using EsbPublisher.Annotations;
using EsbPublisher.Processing.Parse;

namespace EsbPublisher.Controls
{
    /// <summary>
    /// Логика взаимодействия для ParseMatrix24X31Control.xaml
    /// </summary>
    public partial class ParseMatrix24X31Control : UserControl, INotifyPropertyChanged
    {
        private ParseMatrix24X31Logic _logic;

        public ParseMatrix24X31Logic Logic
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

        public ParseMatrix24X31Control(ParseMatrix24X31Logic logic)
        {
            InitializeComponent();
            Logic = logic;
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
