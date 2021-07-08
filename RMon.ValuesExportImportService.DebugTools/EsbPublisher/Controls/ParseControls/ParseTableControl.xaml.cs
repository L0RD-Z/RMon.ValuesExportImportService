using EsbPublisher.Processing.Parse;

namespace EsbPublisher.Controls.ParseControls
{
    /// <summary>
    /// Логика взаимодействия для ParseTableControl.xaml
    /// </summary>
    public partial class ParseTableControl : ParseControlBase
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
    }
}
