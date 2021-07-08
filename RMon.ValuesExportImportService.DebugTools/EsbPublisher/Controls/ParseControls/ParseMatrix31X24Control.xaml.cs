using EsbPublisher.Processing.Parse;

namespace EsbPublisher.Controls.ParseControls
{
    /// <summary>
    /// Логика взаимодействия для ParseMatrix24X31Control.xaml
    /// </summary>
    public partial class ParseMatrix31X24Control : ParseControlBase
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
    }
}
