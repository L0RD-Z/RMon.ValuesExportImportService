using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RMon.Values.ExportImport.Core;
using RMon.Values.ExportImport.Core.FileFormatParameters;
using RMon.ValuesExportImportService.Common;
using RMon.ValuesExportImportService.Data;
using RMon.ValuesExportImportService.Excel;
using RMon.ValuesExportImportService.Extensions;
using RMon.ValuesExportImportService.Files;
using RMon.ValuesExportImportService.Processing.Parse.Format80020;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Processing.Parse
{
    class ParseFlexibleLogic
    {
        private readonly IDataRepository _dataRepository;
        private readonly IExcelWorker _excelWorker;


        public ParseFlexibleLogic(IDataRepository dataRepository, IExcelWorker excelWorker)
        {
            _dataRepository = dataRepository;
            _excelWorker = excelWorker;
        }

        /// <summary>
        /// Асинхронно выполняет анализ файлов в формате 80020
        /// </summary>
        /// <param name="files">Список файлов</param>
        /// <param name="taskParams">Параметры задания</param>
        /// <param name="context">Контекст выполнения</param>
        /// <param name="ct">Токен отмены опреации</param>
        /// <returns></returns>
        public async Task<List<ValueInfo>> AnalyzeFlexibleAsync(IList<LocalFile> files, TableParsingParameters taskParams, ParseProcessingContext context, CancellationToken ct)
        {
            var tables = new List<(string fileName, ImportTable)>();
            foreach (var file in files)
            {
                await context.LogInfo(TextParse.ReadingFile.With(file.Path, ValuesParseFileFormatType.Flexible.ToString())).ConfigureAwait(false);

                var table = _excelWorker.ReadWorksheet(file.Body);
                tables.Add((file.Path, table));
            }

            var result = new List<ValueInfo>();

            return result;
        }
    }
}
