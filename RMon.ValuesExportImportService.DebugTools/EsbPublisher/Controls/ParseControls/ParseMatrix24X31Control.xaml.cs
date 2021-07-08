using EsbPublisher.Processing.Parse;

namespace EsbPublisher.Controls.ParseControls
{
    /// <summary>
    /// Логика взаимодействия для ParseMatrix24X31Control.xaml
    /// </summary>
    public partial class ParseMatrix24X31Control : ParseControlBase
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
        
    }
}
