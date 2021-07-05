using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RMon.Values.ExportImport.Core;
using RMon.Values.ExportImport.Core.FileFormatParameters;
using RMon.ValuesExportImportService.Excel.Common;
using RMon.ValuesExportImportService.Excel.Table;
using RMon.ValuesExportImportService.Extensions;
using RMon.ValuesExportImportService.Files;
using RMon.ValuesExportImportService.Processing.Common;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Processing.Parse
{
    class ParseTableLogic
    {
        private readonly DbValuesAnalyzer _dbValuesAnalyzer;
        private readonly ITableReader _tableReader;

        public ParseTableLogic(DbValuesAnalyzer dbValuesAnalyzer, ITableReader tableReader)
        {
            _dbValuesAnalyzer = dbValuesAnalyzer;
            _tableReader = tableReader;
        }

        public async Task<List<ValueInfo>> AnalyzeAsync(IList<LocalFile> files, TableParsingParameters taskParams, ParseProcessingContext context, CancellationToken ct)
        {
            ValidateParameters(taskParams);
            var logicDevicePropertyValueRow = int.Parse(taskParams.LogicDevicePropertyRow);
            var cellStart = ExcelCellAddressConverter.CellAddressConvert(taskParams.FirstValueCell);
            var dateColumnNumber = ExcelCellAddressConverter.ColNumberConvert(taskParams.DateColumn);
            var timeColumnNumber = ExcelCellAddressConverter.ColNumberConvert(taskParams.TimeColumn);

            var excelResults = new List<ExcelResult>();
            foreach (var file in files)
            {
                await context.LogInfo(TextParse.ReadingFile.With(file.Path, ValuesParseFileFormatType.Table.ToString())).ConfigureAwait(false);

                var message = _tableReader.ReadExcelBook(file.Body, logicDevicePropertyValueRow, cellStart, dateColumnNumber, timeColumnNumber, context);
                if (!message.Any())
                    throw new TaskException(TextParse.ReadFileError.With(file.Path));
                excelResults.Add(new(file.Path, message));
            }

            return await _dbValuesAnalyzer.Analyze(excelResults, taskParams.LogicDevicePropertyCode, taskParams.TagCode, context, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Выполняет валидацию полученных параметров задания
        /// </summary>
        /// <param name="taskParams"></param>
        private void ValidateParameters(TableParsingParameters taskParams)
        {
            if (string.IsNullOrEmpty(taskParams.LogicDevicePropertyCode))
                throw new TaskException(TextParse.MissingLogicDevicePropertyCode);
            if (string.IsNullOrEmpty(taskParams.LogicDevicePropertyRow))
                throw new TaskException(TextParse.MissingLogicDevicePropertyRowNumber);
            if (!int.TryParse(taskParams.LogicDevicePropertyRow, out _))
                throw new TaskException(TextParse.IncorrectLogicDevicePropertyRowNumber);
            if (string.IsNullOrEmpty(taskParams.TagCode))
                throw new TaskException(TextParse.MissingTagCode);
            if (string.IsNullOrEmpty(taskParams.FirstValueCell))
                throw new TaskException(TextParse.MissingFirstValueCellAddress);
            if (string.IsNullOrEmpty(taskParams.DateColumn))
                throw new TaskException(TextParse.MissingDateColumnNumber);
            if (string.IsNullOrEmpty(taskParams.TimeColumn))
                throw new TaskException(TextParse.MissingTimeColumnNumber);
        }
    }
}
