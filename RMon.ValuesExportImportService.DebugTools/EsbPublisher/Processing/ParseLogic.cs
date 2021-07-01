using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EsbPublisher.Annotations;
using EsbPublisher.Processing.Parse;
using EsbPublisher.ServiceBus;
using RMon.Values.ExportImport.Core;
using RMon.Values.ExportImport.Core.FileFormatParameters;

namespace EsbPublisher.Processing
{
    public class ParseLogic : INotifyPropertyChanged
    {
        private readonly BusService _busService;

        private List<ValuesParseFileFormatType> _supportedFileTypes;
        private ValuesParseFileFormatType _selectedFileType;
        private string _filePath;
        private long _idUser;

        private Guid _correlationId;
        private ParseMatrix24X31Logic _matrix24X31Logic;
        private ParseXml80020Logic _xml80020Logic;

        public ParseXml80020Logic Xml80020Logic
        {
            get => _xml80020Logic;
            set
            {
                if (_xml80020Logic != value)
                {
                    _xml80020Logic = value;
                    OnPropertyChanged(nameof(Xml80020Logic));
                }
            }
        }

        public ParseMatrix24X31Logic Matrix24X31Logic
        {
            get => _matrix24X31Logic;
            set
            {
                if (_matrix24X31Logic != value)
                {
                    _matrix24X31Logic = value;
                    OnPropertyChanged(nameof(Matrix24X31Logic));
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
                    OnChangeParseType(value);
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
            Xml80020Logic = new ParseXml80020Logic();
            Matrix24X31Logic = new ParseMatrix24X31Logic();
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

            Xml80020Logic.InitializeProperties();
            Matrix24X31Logic.InitializeProperties();
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
                ValuesParseFileFormatType.Matrix24X31 => SendParseMatrix24X31Async(),
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
            if (Xml80020Logic.MeasuringPoint.Channels.Any())
                parsingParams.MeasuringPoint = new Xml80020PointParameters(Xml80020Logic.MeasuringPoint.PropertyCode)
                {
                    Channels = Xml80020Logic.MeasuringPoint.Channels.Select(t => new Xml80020ChannelParameters(t.ChannelCode, t.TagCode)).ToList()
                };
            if (Xml80020Logic.DeliveryPoint.Channels.Any())
                parsingParams.DeliveryPoint = new Xml80020PointParameters(Xml80020Logic.MeasuringPoint.PropertyCode)
                {
                    Channels = Xml80020Logic.MeasuringPoint.Channels.Select(t => new Xml80020ChannelParameters(t.ChannelCode, t.TagCode)).ToList()
                };

            return _busService.Publisher.SendParseTaskAsync(_correlationId, FilePath, SelectedFileType, parsingParams, IdUser);
        }

        /// <summary>
        /// Отправляет задание на парсинг матрицы 24x31
        /// </summary>
        /// <returns></returns>
        private Task SendParseMatrix24X31Async()
        {
            _correlationId = Guid.NewGuid();

            var parsingParams = new Matrix24X31ParsingParameters()
            {
                LogicDevicePropertyCode = Matrix24X31Logic.LogicDevicePropertyCode,
                LogicDevicePropertyCell = Matrix24X31Logic.LogicDevicePropertyCell,
                TagCode = Matrix24X31Logic.TagCode,
                FirstValueCell = Matrix24X31Logic.FirstValueCell,
                DateColumn = Matrix24X31Logic.DateColumn,
                TimeRow = Matrix24X31Logic.TimeRow.ToString()
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

        public event EventHandler<ValuesParseFileFormatType> ChangeParseType;
        protected virtual void OnChangeParseType(ValuesParseFileFormatType e)
        {
            ChangeParseType?.Invoke(this, e);
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
