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
    public partial class ParseMatrix31X24Control : UserControl, INotifyPropertyChanged
    {
        private ParseMatrix31X24Logic _logic;

        public ParseMatrix31X24Logic Logic
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

        public ParseMatrix31X24Control(ParseMatrix31X24Logic logic)
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
