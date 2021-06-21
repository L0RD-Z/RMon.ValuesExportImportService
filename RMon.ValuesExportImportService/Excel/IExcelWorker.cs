using RMon.Globalization;
using RMon.ValuesExportImportService.Common;

namespace RMon.ValuesExportImportService.Excel
{
    interface IExcelWorker
    {
        /// <summary>
        /// Создаёт книгу Excel и заполняет данными <see cref="exportTable"/>
        /// </summary>
        /// <param name="exportTable">Список оборудования</param>
        /// <param name="globalizationProvider"></param>
        byte[] WriteWorksheet(ExportTable exportTable, IGlobalizationProvider globalizationProvider);
    }
}