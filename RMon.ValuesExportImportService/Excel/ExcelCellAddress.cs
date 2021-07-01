using System;

namespace RMon.ValuesExportImportService.Excel
{
    class ExcelCellAddress : IEquatable<ExcelCellAddress>
    {
        /// <summary>
        /// Номер столбца
        /// </summary>
        public int ColNumber { get; set; }
        /// <summary>
        /// Номер строки
        /// </summary>
        public int RowNumber { get; set; }

        public ExcelCellAddress()
        {
            
        }

        public ExcelCellAddress(int colNumber, int rowNumber)
        {
            ColNumber = colNumber;
            RowNumber = rowNumber;
        }

        #region IEquatable

        public bool Equals(ExcelCellAddress other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ColNumber == other.ColNumber && RowNumber == other.RowNumber;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ExcelCellAddress) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ColNumber, RowNumber);
        }

        #endregion
    }
}
