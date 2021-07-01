using System.ComponentModel;
using System.Runtime.CompilerServices;
using EsbPublisher.Annotations;

namespace EsbPublisher.Processing.Parse
{
    public class ParseMatrix24X31Logic : INotifyPropertyChanged
    {
        private string _logicDevicePropertyCode;
        private string _logicDevicePropertyCell;
        private string _tagCode;
        private string _firstValueCell;
        private string _dateColumn;
        private int _timeRow;

        /// <summary>
        /// Код свойства оборудования
        /// </summary>
        public string LogicDevicePropertyCode
        {
            get => _logicDevicePropertyCode;
            set
            {
                if (_logicDevicePropertyCode != value)
                {
                    _logicDevicePropertyCode = value;
                    OnPropertyChanged(nameof(LogicDevicePropertyCode));
                }
            }
        }

        /// <summary>
        /// Адрес ячейки в файле Excel со значением свойства оборудования
        /// </summary>
        public string LogicDevicePropertyCell
        {
            get => _logicDevicePropertyCell;
            set
            {
                if (_logicDevicePropertyCell != value)
                {
                    _logicDevicePropertyCell = value;
                    OnPropertyChanged(nameof(LogicDevicePropertyCell));
                }
            }
        }

        /// <summary>
        /// Код тега
        /// </summary>
        public string TagCode
        {
            get => _tagCode;
            set
            {
                if (_tagCode != value)
                {
                    _tagCode = value;
                    OnPropertyChanged(nameof(TagCode));
                }
            }
        }

        /// <summary>
        /// Адрес левой верхней ячейки в файле excel
        /// </summary>
        public string FirstValueCell
        {
            get => _firstValueCell;
            set
            {
                if (_firstValueCell != value)
                {
                    _firstValueCell = value;
                    OnPropertyChanged(nameof(FirstValueCell));
                }
            }
        }

        /// <summary>
        /// Номер столбца с датами
        /// </summary>
        public string DateColumn
        {
            get => _dateColumn;
            set
            {
                if (_dateColumn != value)
                {
                    _dateColumn = value;
                    OnPropertyChanged(nameof(DateColumn));
                }
            }
        }

        /// <summary>
        /// Номер строки с часами
        /// </summary>
        public int TimeRow
        {
            get => _timeRow;
            set
            {
                if (_timeRow != value)
                {
                    _timeRow = value;
                    OnPropertyChanged(nameof(TimeRow));
                }
            }
        }


        public void InitializeProperties()
        {
            LogicDevicePropertyCode = "AgrNo";
            LogicDevicePropertyCell = "H3";
            TagCode = "dHHA+";
            FirstValueCell = "C14";
            DateColumn = "A";
            TimeRow = 13;
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
