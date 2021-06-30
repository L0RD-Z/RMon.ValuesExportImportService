using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EsbPublisher.Data;
using EsbPublisher.ServiceBus;

namespace EsbPublisher
{
    public class MainLogic : INotifyPropertyChanged
    {
        private readonly BusService _busService;
        private readonly DataRepository _dataRepository;


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
            _busService = new BusService();
            _busService.StartAsync().Wait();

            _dataRepository = new DataRepository();

            ExportLogic = new ExportLogic(_busService, _dataRepository);
            ParseLogic = new ParseLogic(_busService);


            InitializeProperties();
        }

        private void InitializeProperties()
        {
            ExportLogic.InitializeProperties();
            ParseLogic.InitializeProperties();
        }



        /// <summary>
        /// Отправляет задание на Импорт
        /// </summary>
        /// <returns></returns>
        public async Task SendImportTaskAsync()
        {
            //return SelectedFileType switch
            //{
            //    ValuesParseFileFormatType.Xml80020 => SendParseXml80020(),
            //    ValuesParseFileFormatType.Matrix24X31 => throw new NotImplementedException(),
            //    ValuesParseFileFormatType.Matrix31X24 => throw new NotImplementedException(),
            //    ValuesParseFileFormatType.Table => throw new NotImplementedException(),
            //    ValuesParseFileFormatType.Flexible => throw new NotImplementedException()
            //};
        }

        


        public async Task SendImportTaskAbortAsync()
        {
            //if (_importCorrelationId != Guid.Empty)
            //{
            //    await _publisher.SendImportTaskAbortAsync(_importCorrelationId).ConfigureAwait(false);
            //    ImportTaskAbortEnabled = false;
            //    _importCorrelationId = Guid.Empty;
            //}
            return;
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
