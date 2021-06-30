using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EsbPublisher.Annotations;
using EsbPublisher.ServiceBus;
using RMon.Values.ExportImport.Core;
using EsbPublisher.Model;
using RMon.Values.ExportImport.Core.FileFormatParameters;

namespace EsbPublisher
{
    public class ParseLogic : INotifyPropertyChanged
    {
        private readonly BusService _busService;

        private List<ValuesParseFileFormatType> _supportedFileTypes;
        private ValuesParseFileFormatType _selectedFileType;
        private string _filePath;
        private long _idUser;

        private Guid _correlationId;

        private Point _measuringPoint;
        private Point _deliveryPoint;
        

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

        /// <summary>
        /// Точка измерения
        /// </summary>
        public Point MeasuringPoint
        {
            get => _measuringPoint;
            set
            {
                if (_measuringPoint != value)
                {
                    _measuringPoint = value;
                    OnPropertyChanged(nameof(MeasuringPoint));
                }
            }
        }

        /// <summary>
        /// Точка поставки
        /// </summary>
        public Point DeliveryPoint
        {
            get => _deliveryPoint;
            set
            {
                if (_deliveryPoint != value)
                {
                    _deliveryPoint = value;
                    OnPropertyChanged(nameof(DeliveryPoint));
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


        public ParseLogic(BusService busService)
        {
            _busService = busService;
            MeasuringPoint = new Point();
            DeliveryPoint = new Point();
        }


        public void InitializeProperties()
        {
            IdUser = 14;

            SupportedFileTypes = new List<ValuesParseFileFormatType>
            {
                ValuesParseFileFormatType.Xml80020,
                ValuesParseFileFormatType.Matrix24X31,
                ValuesParseFileFormatType.Matrix31X24,
                ValuesParseFileFormatType.Table,
                ValuesParseFileFormatType.Flexible,
            };

            MeasuringPoint = new Point("AgrNo")
            {
                Channels = new ObservableCollection<ChannelMap>
                {
                    new("01", "dHHA+")
                }
            };
        }

        /// <summary>
        /// Отправляет задания на Парсинг
        /// </summary>
        /// <returns></returns>
        public Task SendTaskAsync()
        {
            return SelectedFileType switch
            {
                ValuesParseFileFormatType.Xml80020 => SendParseXml80020Async(),
                ValuesParseFileFormatType.Matrix24X31 => throw new NotImplementedException(),
                ValuesParseFileFormatType.Matrix31X24 => throw new NotImplementedException(),
                ValuesParseFileFormatType.Table => throw new NotImplementedException(),
                ValuesParseFileFormatType.Flexible => SendParseFlexibleAsync(),
                _ => throw new NotImplementedException()
            };
        }

        

        /// <summary>
        /// Отправляет задание на Отмену парсинга
        /// </summary>
        /// <returns></returns>
        public async Task SendTaskAbortAsync()
        {
            if (_correlationId != Guid.Empty)
            {
                await _busService.Publisher.SendParseTaskAbortAsync(_correlationId).ConfigureAwait(false);
                _correlationId = Guid.Empty;
            }
        }

        /// <summary>
        /// Отправляет задание на парсинг формата 80020
        /// </summary>
        /// <returns></returns>
        private Task SendParseXml80020Async()
        {
            _correlationId = Guid.NewGuid();

            var parsingParams = new Xml80020ParsingParameters();
            if (MeasuringPoint.Channels.Any())
                parsingParams.MeasuringPoint = new Xml80020PointParameters(MeasuringPoint.PropertyCode)
                {
                    Channels = MeasuringPoint.Channels.Select(t => new Xml80020ChannelParameters(t.ChannelCode, t.TagCode)).ToList()
                };
            if (DeliveryPoint.Channels.Any())
                parsingParams.DeliveryPoint = new Xml80020PointParameters(MeasuringPoint.PropertyCode)
                {
                    Channels = MeasuringPoint.Channels.Select(t => new Xml80020ChannelParameters(t.ChannelCode, t.TagCode)).ToList()
                };

            return _busService.Publisher.SendParseTaskAsync(_correlationId, FilePath, SelectedFileType, parsingParams, IdUser);
        }

        /// <summary>
        /// Отправляет задание на парсинг "Гибкого формата"
        /// </summary>
        /// <returns></returns>
        private Task SendParseFlexibleAsync()
        {
            _correlationId = Guid.NewGuid();

            return _busService.Publisher.SendParseTaskAsync(_correlationId, FilePath, SelectedFileType, IdUser);
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
