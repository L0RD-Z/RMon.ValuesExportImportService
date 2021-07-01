using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
