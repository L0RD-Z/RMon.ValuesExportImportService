using System.Collections.Generic;
using System.Threading.Tasks;
using RMon.Globalization;
using RMon.ValuesExportImportService.Common;
using RMon.ValuesExportImportService.Excel.Common;
using RMon.ValuesExportImportService.Processing.Parse;

namespace RMon.ValuesExportImportService.Excel
{
    interface IExcelWorker
    {
        /// <summary>
        /// Создаёт книгу Excel и заполняет данными <see cref="exportTable"/>
        /// </summary>
        /// <param name="exportTable">Список оборудования</param>
        /// <param name="globalizationProvider"></param>
        byte[] WriteFile(ExportTable exportTable, IGlobalizationProvider globalizationProvider);

        /// <summary>
        /// Выполняет парсинг файла Excel-файла <see cref="fileBody"/>
        /// </summary>
        /// <param name="fileBody">Файл excel</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<List<ReadedSheet>> ReadFile(byte[] fileBody, ParseProcessingContext context);
    }
}