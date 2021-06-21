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
using RMon.Values.ExportImport.Core;

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
        private Guid _parseCorrelationId;
        private Guid _importCorrelationId;
        private long _idUser;
        private List<ValuesParseFileFormatType> _supportedFileTypes;
        private ValuesParseFileFormatType _selectedFileType;
        private string _filePath;

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

        /// <summary>
        /// Поддерживаемые типы файлов
        /// </summary>
        public List<ValuesParseFileFormatType> SupportedFileTypes
        {
            get => _supportedFileTypes;
            set
            {
                if (_supportedFileTypes != value)
                {
                    _supportedFileTypes = value;
                    OnPropertyChanged(nameof(SupportedFileTypes));
                }

            }
        }
        /// <summary>
        /// Выбранный тип файла
        /// </summary>
        public ValuesParseFileFormatType SelectedFileType
        {
            get => _selectedFileType;
            set
            {
                if (_selectedFileType != value)
                {
                    _selectedFileType = value;
                    OnPropertyChanged(nameof(SelectedFileType));
                }
            }
        }

        public string FilePath
        {
            get => _filePath;
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    OnPropertyChanged(nameof(FilePath));
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
            IdUser = 14;
            SupportedFileTypes = new List<ValuesParseFileFormatType>()
            {
                ValuesParseFileFormatType.Xml80020,
                ValuesParseFileFormatType.Matrix24X31,
                ValuesParseFileFormatType.Matrix31X24,
                ValuesParseFileFormatType.Table,
                ValuesParseFileFormatType.Flexible,
            };
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
        public async Task SendExportTaskAsync()
        {
            _exportCorrelationId = Guid.NewGuid();

            var idLogicDevices = new List<long> { 3332, 3940, 3942, 3946, 5582, 6203, 6205, 6213, 6236, 9638, 9671, 57122, 57172, 57174, 57123, 1299, 1301, 1302, 1303, 1335, 1318, 1319, 1320, 1321, 1322 };
            var tagTypeCodes = TagTypes.Where(t => t.IsSelect).Select(t => t.Entity.Code).ToList();
            var propertyCodes = DevicePropertyTypes.Where(t => t.IsSelect).Select(t => t.Entity.Code).ToList();

            await _busService.Publisher.SendExportTaskAsync(_exportCorrelationId, DateStart, DateEnd, idLogicDevices, tagTypeCodes, propertyCodes, IdUser).ConfigureAwait(true);
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
        /// Отправляет задания на Парсинг
        /// </summary>
        /// <returns></returns>
        public Task SendParseTaskAsync()
        {
            return SelectedFileType switch
            {
                ValuesParseFileFormatType.Xml80020 => SendParseXml80020(),
                ValuesParseFileFormatType.Matrix24X31 => throw new NotImplementedException(),
                ValuesParseFileFormatType.Matrix31X24 => throw new NotImplementedException(),
                ValuesParseFileFormatType.Table => throw new NotImplementedException(),
                ValuesParseFileFormatType.Flexible => throw new NotImplementedException(),
                _ => throw new NotImplementedException()
            };
        }

        /// <summary>
        /// Отправляет задание на Отмену парсинга
        /// </summary>
        /// <returns></returns>
        public async Task SendParseTaskAbortAsync()
        {
            if (_parseCorrelationId != Guid.Empty)
            {
                await _busService.Publisher.SendParseTaskAbortAsync(_parseCorrelationId).ConfigureAwait(false);
                _parseCorrelationId = Guid.Empty;
            }
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


        private async Task SendParseXml80020()
        {
            _parseCorrelationId = Guid.NewGuid();
            await _busService.Publisher.SendParseTaskAsync(_parseCorrelationId, FilePath, SelectedFileType, IdUser).ConfigureAwait(false);
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
