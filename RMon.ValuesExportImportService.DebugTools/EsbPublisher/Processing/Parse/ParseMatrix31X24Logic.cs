namespace EsbPublisher.Processing.Parse
{
    public class ParseMatrix31X24Logic : ParseMatrixBase
    {
        private string _timeColumn;
        private int _dateRow;

        
        /// <summary>
        /// Номер строки с датами
        /// </summary>
        public int DateRow
        {
            get => _dateRow;
            set
            {
                if (_dateRow != value)
                {
                    _dateRow = value;
                    OnPropertyChanged(nameof(DateRow));
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
            LogicDevicePropertyCell = "H3";
            TagCode = "dHHA+";
            FirstValueCell = "B15";
            DateRow = 13;
            TimeColumn = "A";
        }
    }
}
