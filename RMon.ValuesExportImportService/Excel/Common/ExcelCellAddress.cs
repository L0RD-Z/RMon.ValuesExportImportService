namespace RMon.ValuesExportImportService.Excel.Common
{
    record ExcelCellAddress
    {
        private int _colNumber;
        private int _rowNumber;

        /// <summary>
        /// Номер столбца (с отсчетом от 1)
        /// </summary>
        public int ColNumber
        {
            get => _colNumber;
            set
            {
                _colNumber = value;
                ColIndex = value - 1;
            }
        }

        /// <summary>
        /// Номер строки (с отсчетом от 1)
        /// </summary>
        public int RowNumber
        {
            get => _rowNumber;
            set
            {
                _rowNumber = value;
                RowIndex = value - 1;
            }
        }

        /// <summary>
        /// Номер столбца (с отсчетом от 0)
        /// </summary>
        public int ColIndex { get; private set; }

        /// <summary>
        /// Номер строки (с отсчетом от 0)
        /// </summary>
        public int RowIndex { get; private set; }


        public ExcelCellAddress()
        {
            
        }

        public ExcelCellAddress(int colNumber, int rowNumber)
        {
            ColNumber = colNumber;
            RowNumber = rowNumber;
        }

    }
}
