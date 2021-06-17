using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace RMon.ValuesExportImportService.Excel.Common
{
    static class ExcelMethods
    {
        /// <summary>
        /// Создаёт и возвращает лист с именем <see cref="name"/>
        /// </summary>
        /// <param name="workbook">Книга Excel, в которой будет создан новый лист</param>
        /// <param name="name">Имя Лист</param>
        /// <returns></returns>
        public static ExcelWorksheet WorksheetCreate(ExcelWorkbook workbook, string name)
        {
            var excelSheet = workbook.Worksheets.Add(name);
            excelSheet.Cells.Style.Font.Size = 10;
            excelSheet.Cells.Style.Font.Name = "Times New Roman";
            return excelSheet;
        }

        /// <summary>
        /// Объединяет ячейки в количестве <see cref="rowCount"/> шт. в пределах одного столбца <see cref="colIndex"/>
        /// </summary>
        /// <param name="excelSheet"></param>
        /// <param name="rowIndex">Номер строки</param>
        /// <param name="colIndex">Номер столбца</param>
        /// <param name="rowCount">Количество ячеек для объединения</param>
        public static void RowMerge(ExcelWorksheet excelSheet, int rowIndex, int colIndex, int rowCount) => excelSheet.Cells[rowIndex, colIndex, rowIndex + rowCount - 1, colIndex].Merge = true;

        /// <summary>
        /// Объединяет ячейки в количестве <see cref="colCount"/> шт. в пределах одной строки <see cref="rowIndex"/>
        /// </summary>
        /// <param name="excelSheet"></param>
        /// <param name="rowIndex">Номер строки</param>
        /// <param name="colIndex">Номер столбца</param>
        /// <param name="colCount">Количество ячеек для объединения</param>
        public static void ColMerge(ExcelWorksheet excelSheet, int rowIndex, int colIndex, int colCount) => excelSheet.Cells[rowIndex, colIndex, rowIndex, colIndex + colCount - 1].Merge = true;

        /// <summary>
        /// Устанавливает стиль границы ячеек <see cref="cells"/>
        /// </summary>
        /// <param name="cells">Диапазон ячеек</param>
        public static void SetBorderStyle(ExcelRange cells)
        {
            cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Left.Color.SetColor(Color.Black);
            cells.Style.Border.Right.Color.SetColor(Color.Black);
            cells.Style.Border.Top.Color.SetColor(Color.Black);
            cells.Style.Border.Bottom.Color.SetColor(Color.Black);
        }
    }
}
