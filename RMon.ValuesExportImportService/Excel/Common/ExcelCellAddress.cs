namespace RMon.ValuesExportImportService.Excel.Common
{
    record ExcelCellAddress
    {
        /// <summary>
        /// Индекс столбца (с отсчетом от 0)
        /// </summary>
        public int ColIndex { get; private set; }

        /// <summary>
        /// Индекс строки (с отсчетом от 0)
        /// </summary>
        public int RowIndex { get; private set; }


        public ExcelCellAddress()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colNumber">Номер столбца (с отсчетом от 1)</param>
        /// <param name="rowNumber">Индекс строки (с отсчетом от 1)</param>
        public ExcelCellAddress(int colNumber, int rowNumber)
        {
            ColIndex = colNumber - 1;
            RowIndex = rowNumber - 1;
        }

    }
}
