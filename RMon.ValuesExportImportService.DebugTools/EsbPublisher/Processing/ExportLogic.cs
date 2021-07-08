using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using EsbPublisher.Annotations;
using EsbPublisher.Data;
using EsbPublisher.Model;
using EsbPublisher.ServiceBus;
using RMon.Data.Provider.Configuration;

namespace EsbPublisher.Processing
{
    public class ExportLogic : INotifyPropertyChanged
    {
        private readonly BusService _busService;
        private readonly DataRepository _dataRepository;

        private DateTime _dateStart;
        private DateTime _dateEnd;
        private long _idUser;
        
        private IList<Selected<DevicePropertyType>> _devicePropertyTypes;
        private IList<Selected<LogicTagType>> _tagTypes;

        private Guid _correlationId;

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


        public long IdUser
        {
            get => _idUser;
            set
            {
                if (_idUser != value)
                {
                    _idUser = value;
                    OnPropertyChanged(nameof(IdUser));
                }
            }
        }


        public ExportLogic(BusService busService, DataRepository dataRepository)
        {
            _busService = busService;
            _dataRepository = dataRepository;

            InitializeProperties();
        }

        public void InitializeProperties()
        {
            DateEnd = DateTime.Now;
            DateStart = DateEnd.AddMonths(-1);
            IdUser = 14;
        }

        public async Task LoadDataAsync()
        {
            var devicePropertyTypes = await _dataRepository.GetDevicePropertyTypesAsync(CancellationToken.None).ConfigureAwait(false);
            var tagTypes = await _dataRepository.GetTagTypesAsync(CancellationToken.None).ConfigureAwait(false);
            DevicePropertyTypes = devicePropertyTypes.Select(t => new Selected<DevicePropertyType>(t)).OrderBy(t => t.Entity.Name).ToList();

            TagTypes = tagTypes.Select(t => new Selected<LogicTagType>(t, true)).OrderBy(t => t.Entity.Name).ToList();
        }


        /// <summary>
        /// Отправляет задание на Экспорт
        /// </summary>
        /// <returns></returns>
        public async Task SendTaskAsync()
        {
            _correlationId = Guid.NewGuid();

            var idLogicDevices = new List<long> { 3332, 3940, 3942, 3946, 5582, 6203, 6205, 6213, 6236, 9638, 9671, 57122, 57172, 57174, 57123, 1299, 1301, 1302, 1303, 1335, 1318, 1319, 1320, 1321, 1322 };
            var tagTypeCodes = TagTypes.Where(t => t.IsSelect).Select(t => t.Entity.Code).ToList();
            var propertyCodes = DevicePropertyTypes.Where(t => t.IsSelect).Select(t => t.Entity.Code).ToList();

            await _busService.Publisher.SendExportTaskAsync(_correlationId, DateStart, DateEnd, idLogicDevices, tagTypeCodes, propertyCodes, IdUser).ConfigureAwait(true);
        }

        /// <summary>
        /// Отправляет задание на Отмену экспорта
        /// </summary>
        /// <returns></returns>
        public async Task SendTaskAbortAsync()
        {
            if (_correlationId != Guid.Empty)
            {
                await _busService.Publisher.SendExportTaskAbortAsync(_correlationId).ConfigureAwait(false);
                _correlationId = Guid.Empty;
            }
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
