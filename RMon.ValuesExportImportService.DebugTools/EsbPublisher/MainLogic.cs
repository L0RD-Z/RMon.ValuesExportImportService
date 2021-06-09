using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EsbPublisher.Data;
using EsbPublisher.Model;
using EsbPublisher.ServiceBus;
using MassTransit;
using RMon.Data.Provider.Configuration;

namespace EsbPublisher
{
    public class MainLogic : INotifyPropertyChanged
    {
        private readonly BusService _busService;
        private readonly DataRepository _dataRepository;

        private DateTime _dateStart;
        private DateTime _dateEnd;
        private IList<Selected<DevicePropertyType>> _devicePropertyTypes;
        private IList<Selected<LogicTagType>> _tagTypes;

        private Guid _exportCorrelationId;

        public IList<Selected<DevicePropertyType>> DevicePropertyTypes
        {
            get => _devicePropertyTypes;
            private set
            {
                _devicePropertyTypes = value;
                OnPropertyChanged(nameof(DevicePropertyTypes));
            }
        }

        public IList<Selected<LogicTagType>> TagTypes
        {
            get => _tagTypes;
            set
            {
                if (_tagTypes != value)
                {
                    _tagTypes = value;
                    OnPropertyChanged(nameof(TagTypes));
                }
            }
        }

        /// <summary>
        /// Начало временного диапазона
        /// </summary>
        public DateTime DateStart
        {
            get => _dateStart;
            set
            {
                if (_dateStart != value)
                {
                    _dateStart = value;
                    OnPropertyChanged(nameof(DateStart));
                }
            }
        }

        /// <summary>
        /// Конец временного диапазона
        /// </summary>
        public DateTime DateEnd
        {
            get => _dateEnd;
            set
            {
                if (_dateEnd != value)
                {
                    _dateEnd = value;
                    OnPropertyChanged(nameof(DateEnd));
                }
            }
        }

        public MainLogic()
        {
            _busService = new BusService();
            _busService.StartAsync().Wait();

            _dataRepository = new DataRepository();


            InitializeProperties();
        }

        void InitializeProperties()
        {
            DateEnd = DateTime.Now;
            DateStart = DateEnd.AddMonths(-1);
        }


        public async Task LoadDataAsync()
        {
            var devicePropertyTypes = await _dataRepository.GetDevicePropertyTypesAsync(CancellationToken.None).ConfigureAwait(false);
            var tagTypes = await _dataRepository.GetTagTypesAsync(CancellationToken.None).ConfigureAwait(false);
            DevicePropertyTypes = devicePropertyTypes.Select(t => new Selected<DevicePropertyType>(t)).OrderBy(t => t.Entity.Name).ToList();
            TagTypes = tagTypes.Select(t => new Selected<LogicTagType>(t)).OrderBy(t => t.Entity.Name).ToList();
        }




        /// <summary>
        /// Отправляет задание на Экспорт
        /// </summary>
        /// <returns></returns>
        public async Task SendExportTaskAsync()
        {
            _exportCorrelationId = Guid.NewGuid();

            var idLogicDevices = new List<long> { 5311 };
            var tagTypeCodes = TagTypes.Where(t => t.IsSelect).Select(t => t.Entity.Code).ToList();
            var propertyCodes = DevicePropertyTypes.Where(t => t.IsSelect).Select(t => t.Entity.Code).ToList();

            await _busService.Publisher.SendExportTaskAsync(_exportCorrelationId, DateStart, DateEnd, idLogicDevices, tagTypeCodes, propertyCodes).ConfigureAwait(true);
        }

        /// <summary>
        /// Отправляет задание на Отмену экспорта
        /// </summary>
        /// <returns></returns>
        public async Task SendExportTaskAbortAsync()
        {
            if (_exportCorrelationId != Guid.Empty)
            {
                await _busService.Publisher.SendExportTaskAbortAsync(_exportCorrelationId).ConfigureAwait(false);
                _exportCorrelationId = Guid.Empty;
            }
        }


        /// <summary>
        /// Отправляет задание на Импорт
        /// </summary>
        /// <returns></returns>
        public async Task SendImportTaskAsync()
        {
            //switch (SelectedImportSettingsType)
            //{
            //    case ServiceBusPublisher.ImportSettingsTypes.ImportFromFiles:
            //        return SendImportFromFilesTaskAsync();
            //    case ServiceBusPublisher.ImportSettingsTypes.ImportFromSiteAts:
            //        return SendImportFromSiteAtsTaskAsync();
            //    default:
            //        throw new ArgumentOutOfRangeException();
            //}
            return;
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
