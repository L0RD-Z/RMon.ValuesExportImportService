using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using EsbPublisher.Annotations;
using EsbPublisher.Processing.Parse;

namespace EsbPublisher.Controls
{
    /// <summary>
    /// Логика взаимодействия для ParseTableControl.xaml
    /// </summary>
    public partial class ParseTableControl : UserControl, INotifyPropertyChanged
    {
        private ParseTableLogic _logic;

        public ParseTableLogic Logic
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

        public ParseTableControl(ParseTableLogic logic)
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
