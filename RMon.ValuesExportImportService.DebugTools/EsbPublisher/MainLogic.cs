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

        public MainLogic()
        {
            var busService = new BusService();
            busService.StartAsync().Wait();

            var dataRepository = new DataRepository();

            ExportLogic = new ExportLogic(busService, dataRepository);
            ParseLogic = new ParseLogic(busService);


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
