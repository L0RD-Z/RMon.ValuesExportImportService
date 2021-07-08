using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EsbPublisher.Annotations;
using EsbPublisher.ServiceBus;
using Newtonsoft.Json;
using RMon.Values.ExportImport.Core;

namespace EsbPublisher.Processing
{
    public class ImportLogic : INotifyPropertyChanged
    {
        private readonly BusService _busService;

        private long _idUser;


        private Guid _correlationId;
        private string _jsonValues = string.Empty;


        public string JsonValues
        {
            get => _jsonValues;
            set
            {
                if (_jsonValues != value)
                {
                    _jsonValues = value;
                    OnPropertyChanged(nameof(JsonValues));
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


        public ImportLogic(BusService busService)
        {
            _busService = busService;

            InitializeProperties();
        }

        public void InitializeProperties()
        {
            IdUser = 14;
            
            if (File.Exists("Values.json"))
                JsonValues = File.ReadAllText("Values.json");
        }



        /// <summary>
        /// Отправляет задание на Экспорт
        /// </summary>
        /// <returns></returns>
        public async Task SendTaskAsync()
        {
            _correlationId = Guid.NewGuid();

            var values = JsonConvert.DeserializeObject<ValueInfo[]>(JsonValues);

            await _busService.Publisher.SendImportTaskAsync(_correlationId, values, IdUser).ConfigureAwait(true);
        }

        /// <summary>
        /// Отправляет задание на Отмену экспорта
        /// </summary>
        /// <returns></returns>
        public async Task SendTaskAbortAsync()
        {
            if (_correlationId != Guid.Empty)
            {
                await _busService.Publisher.SendImportTaskAbortAsync(_correlationId).ConfigureAwait(false);
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
