using System.ComponentModel;
using System.Runtime.CompilerServices;
using EsbPublisher.Annotations;

namespace EsbPublisher.Processing.Parse
{
    public class ParseTableLogic : INotifyPropertyChanged
    {
        private string _logicDevicePropertyCode;
        private string _logicDevicePropertyRow;
        private string _tagCode;
        private string _firstValueCell;
        private string _dateColumn;
        private string _timeColumn;

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
        /// Номер строки со значениями свойств оборудования
        /// </summary>
        public string LogicDevicePropertyRow
        {
            get => _logicDevicePropertyRow;
            set
            {
                if (_logicDevicePropertyRow != value)
                {
                    _logicDevicePropertyRow = value;
                    OnPropertyChanged(nameof(LogicDevicePropertyRow));
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
        /// Номер столбца с часами
        /// </summary>
        public string TimeColumn
        {
            get => _timeColumn;
            set
            {
                if (_timeColumn != value)
                {
                    _timeColumn = value;
                    OnPropertyChanged(nameof(TimeColumn));
                }
            }
        }

        public void InitializeProperties()
        {
            LogicDevicePropertyCode = "AgrNo";
            LogicDevicePropertyRow = "2";
            TagCode = "dHHA+";
            FirstValueCell = "C3";
            DateColumn = "A1";
            TimeColumn = "B1";
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
