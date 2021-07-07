namespace EsbPublisher.Processing.Parse
{
    public class ParseMatrix24X31Logic : ParseMatrixBase
    {
        private string _dateColumn;
        private int _timeRow;

        
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
    }
}
