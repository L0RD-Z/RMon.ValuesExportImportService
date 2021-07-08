using System.ComponentModel;
using System.Runtime.CompilerServices;
using EsbPublisher.Data;
using EsbPublisher.Processing;
using EsbPublisher.ServiceBus;

namespace EsbPublisher
{
    public class MainLogic : INotifyPropertyChanged
    {
        private ExportLogic _exportLogic;
        private ParseLogic _parseLogic;
        private ImportLogic _importLogic;


        public ExportLogic ExportLogic
        {
            get => _exportLogic;
            set
            {
                if (_exportLogic != value)
                {
                    _exportLogic = value;
                    OnPropertyChanged(nameof(ExportLogic));
                }
            }
        }
        public ParseLogic ParseLogic
        {
            get => _parseLogic;
            set
            {
                if (_parseLogic != value)
                {
                    _parseLogic = value;
                    OnPropertyChanged(nameof(ParseLogic));
                }
            }
        }
        public ImportLogic ImportLogic
        {
            get => _importLogic;
            set
            {
                if (_importLogic != value)
                {
                    _importLogic = value;
                    OnPropertyChanged(nameof(ImportLogic));
                }
            }
        }


        public MainLogic()
        {
            var busService = new BusService();
            busService.StartAsync().Wait();

            var dataRepository = new DataRepository();

            ExportLogic = new ExportLogic(busService, dataRepository);
            ParseLogic = new ParseLogic(busService);
            ImportLogic = new ImportLogic(busService);

            InitializeProperties();
        }

        private void InitializeProperties()
        {
            ExportLogic.InitializeProperties();
            ParseLogic.InitializeProperties();
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
