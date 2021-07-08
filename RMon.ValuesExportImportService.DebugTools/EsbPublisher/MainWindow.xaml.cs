using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using EsbPublisher.Controls;
using EsbPublisher.Controls.OperationsControl;
using EsbPublisher.Model;

namespace EsbPublisher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private MainLogic _logic;
        private ObservableCollection<Operations> _operations;
        private Operations _selectedOperation;
        private readonly Dictionary<Operations, OperationControlBase> _operationControls;
        private OperationControlBase _selectedOperationControl;

        

        public MainLogic Logic
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

        public ObservableCollection<Operations> Operations
        {
            get => _operations;
            set
            {
                if (_operations != value)
                {
                    _operations = value;
                    OnPropertyChanged(nameof(Operations));
                }
            }
        }

        public Operations SelectedOperation
        {
            get => _selectedOperation;
            set
            {
                if (_selectedOperation != value)
                {
                    _selectedOperation = value;
                    OnPropertyChanged(nameof(SelectedOperation));
                    SelectedOperationControl = _operationControls[value];
                }
            }
        }

        public OperationControlBase SelectedOperationControl
        {
            get => _selectedOperationControl;
            set
            {
                if (_selectedOperationControl != value)
                {
                    _selectedOperationControl = value;
                    OnPropertyChanged(nameof(SelectedOperationControl));
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            Logic = new MainLogic();

            _operationControls = new Dictionary<Operations, OperationControlBase>()
            {
                {Model.Operations.Export, new ExportControl(Logic.ExportLogic)},
                {Model.Operations.Parse, new ParseControl(Logic.ParseLogic)},
                {Model.Operations.Import, new ImportControl(Logic.ImportLogic)},
            };

            Operations = new ObservableCollection<Operations>()
            {
                Model.Operations.Export,
                Model.Operations.Parse,
                Model.Operations.Import,
            };
            SelectedOperation = Operations.First();
            SelectedOperationControl = _operationControls[SelectedOperation];
        }



        

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        
    }
}
