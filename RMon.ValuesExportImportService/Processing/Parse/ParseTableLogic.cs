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
            var logicDevicePropertyValueRowIndex = ExcelCellAddressConverter.ExcelRowToIndex(taskParams.LogicDevicePropertyRow);
            var cellStart = ExcelCellAddressConverter.CellAddressConvert(taskParams.FirstValueCell);
            var dateColumnIndex = ExcelCellAddressConverter.ExcelColumnToIndex(taskParams.DateColumn);
            var timeColumnIndex = ExcelCellAddressConverter.ExcelColumnToIndex(taskParams.TimeColumn);

            var excelResults = new List<ExcelResult>();
            foreach (var file in files)
            {
                await context.LogInfo(TextParse.ReadingFile.With(file.Name, ValuesParseFileFormatType.Table.ToString())).ConfigureAwait(false);

                var excelResult = _tableReader.ReadExcelBook(file, logicDevicePropertyValueRowIndex, cellStart, dateColumnIndex, timeColumnIndex, context);
                if (!excelResult.Any())
                    throw new TaskException(TextParse.ReadFileError.With(file.Name));
                excelResults.Add(new(file.Name, excelResult));
            }

            return await _dbValuesAnalyzer.Analyze(excelResults, taskParams.LogicDevicePropertyCode, taskParams.TagCode, context, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Выполняет валидацию полученных параметров задания
        /// </summary>
        /// <param name="taskParams"></param>
        private static void ValidateParameters(TableParsingParameters taskParams)
        {
            if (string.IsNullOrEmpty(taskParams.LogicDevicePropertyCode))
                throw new TaskException(TextParse.MissingLogicDevicePropertyCodeError);
            if (string.IsNullOrEmpty(taskParams.LogicDevicePropertyRow))
                throw new TaskException(TextParse.MissingLogicDevicePropertyRowNumberError);
            if (!int.TryParse(taskParams.LogicDevicePropertyRow, out _))
                throw new TaskException(TextParse.IncorrectLogicDevicePropertyRowNumberError);
            if (string.IsNullOrEmpty(taskParams.TagCode))
                throw new TaskException(TextParse.MissingTagCodeError);
            if (string.IsNullOrEmpty(taskParams.FirstValueCell))
                throw new TaskException(TextParse.MissingFirstValueCellAddressError);
            if (string.IsNullOrEmpty(taskParams.DateColumn))
                throw new TaskException(TextParse.MissingDateColumnNumberError);
            if (string.IsNullOrEmpty(taskParams.TimeColumn))
                throw new TaskException(TextParse.MissingTimeColumnNumberError);

            var cellStart = ExcelCellAddressConverter.CellAddressConvert(taskParams.FirstValueCell);
            var logicDevicePropertyValueRowIndex = ExcelCellAddressConverter.ExcelRowToIndex(taskParams.LogicDevicePropertyRow);
            if (cellStart.RowIndex <= logicDevicePropertyValueRowIndex)
                throw new TaskException(TextParse.IncorrectRowStartError);
        }
    }
}
