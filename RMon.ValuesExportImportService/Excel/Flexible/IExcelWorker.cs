using System.Collections.Generic;
using System.Threading.Tasks;
using RMon.Globalization;
using RMon.ValuesExportImportService.Common;
using RMon.ValuesExportImportService.Excel.Common;
using RMon.ValuesExportImportService.Files;
using RMon.ValuesExportImportService.Processing.Parse;

namespace RMon.ValuesExportImportService.Excel.Flexible
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
        /// Выполняет парсинг Excel-файла <see cref="file"/>
        /// </summary>
        /// <param name="file">Файл excel</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<List<ReadSheet>> ReadFile(LocalFile file, ParseProcessingContext context);
    }
}