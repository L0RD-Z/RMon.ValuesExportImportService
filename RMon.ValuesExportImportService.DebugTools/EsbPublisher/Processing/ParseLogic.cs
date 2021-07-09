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

        private ParseXml80020Logic _xml80020Logic;
        private ParseMatrix24X31Logic _matrix24X31Logic;
        private ParseMatrix31X24Logic _matrix31X24Logic;
        private ParseTableLogic _tableLogic;
        private List<ValuesParseFileFormatType> _supportedFileTypes;
        private ValuesParseFileFormatType _selectedFileType;
        private string _filePath;
        private long _idUser;

        private Guid _correlationId;
        private bool _useTransformationRatio;


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
        public ParseMatrix31X24Logic Matrix31X24Logic
        {
            get => _matrix31X24Logic;
            set
            {
                if (_matrix31X24Logic != value)
                {
                    _matrix31X24Logic = value;
                    OnPropertyChanged(nameof(Matrix31X24Logic));
                }
            }
        }
        public ParseTableLogic TableLogic
        {
            get => _tableLogic;
            set
            {
                if (_tableLogic != value)
                {
                    _tableLogic = value;
                    OnPropertyChanged(nameof(TableLogic));
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

        /// <summary>
        /// <see langword="True"/>, если при парсинге нужно применять коэффициенты трансформации
        /// </summary>
        public bool UseTransformationRatio
        {
            get => _useTransformationRatio;
            set
            {
                if (_useTransformationRatio != value)
                {
                    _useTransformationRatio = value;
                    OnPropertyChanged(nameof(UseTransformationRatio));
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
            Matrix31X24Logic = new ParseMatrix31X24Logic();
            TableLogic = new ParseTableLogic();
        }


        public void InitializeProperties()
        {
            IdUser = 14;
            UseTransformationRatio = true;

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
            Matrix31X24Logic.InitializeProperties();
            TableLogic.InitializeProperties();
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
                ValuesParseFileFormatType.Matrix31X24 => SendParseMatrix31X24Async(),
                ValuesParseFileFormatType.Table => SendParseTableAsync(),
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

            return _busService.Publisher.SendParseTaskAsync(_correlationId, FilePath, SelectedFileType, UseTransformationRatio, parsingParams, IdUser);
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

            return _busService.Publisher.SendParseTaskAsync(_correlationId, FilePath, SelectedFileType, UseTransformationRatio, parsingParams, IdUser);
        }

        /// <summary>
        /// Отправляет задание на парсинг матрицы 31x24
        /// </summary>
        /// <returns></returns>
        private Task SendParseMatrix31X24Async()
        {
            _correlationId = Guid.NewGuid();

            var parsingParams = new Matrix31X24ParsingParameters()
            {
                LogicDevicePropertyCode = Matrix31X24Logic.LogicDevicePropertyCode,
                LogicDevicePropertyCell = Matrix31X24Logic.LogicDevicePropertyCell,
                TagCode = Matrix31X24Logic.TagCode,
                FirstValueCell = Matrix31X24Logic.FirstValueCell,
                DateRow = Matrix31X24Logic.DateRow.ToString(),
                TimeColumn = Matrix31X24Logic.TimeColumn
            };

            return _busService.Publisher.SendParseTaskAsync(_correlationId, FilePath, SelectedFileType, UseTransformationRatio, parsingParams, IdUser);
        }

        /// <summary>
        /// Отправляет задание на парсинг Таблицы
        /// </summary>
        /// <returns></returns>
        private Task SendParseTableAsync()
        {
            _correlationId = Guid.NewGuid();
            var parsingParams = new TableParsingParameters
            {
                LogicDevicePropertyCode = TableLogic.LogicDevicePropertyCode,
                LogicDevicePropertyRow = TableLogic.LogicDevicePropertyRow.ToString(),
                TagCode = TableLogic.TagCode,
                FirstValueCell = TableLogic.FirstValueCell,
                DateColumn = TableLogic.DateColumn,
                TimeColumn = TableLogic.TimeColumn
            };

            return _busService.Publisher.SendParseTaskAsync(_correlationId, FilePath, SelectedFileType, UseTransformationRatio, parsingParams, IdUser);
        }

        /// <summary>
        /// Отправляет задание на парсинг "Гибкого формата"
        /// </summary>
        /// <returns></returns>
        private Task SendParseFlexibleAsync()
        {
            _correlationId = Guid.NewGuid();

            return _busService.Publisher.SendParseTaskAsync(_correlationId, FilePath, SelectedFileType, UseTransformationRatio, IdUser);
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
