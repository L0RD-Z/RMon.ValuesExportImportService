﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RMon.Values.ExportImport.Core;
using RMon.Values.ExportImport.Core.FileFormatParameters;
using RMon.ValuesExportImportService.Excel.Common;
using RMon.ValuesExportImportService.Excel.Matrix;
using RMon.ValuesExportImportService.Extensions;
using RMon.ValuesExportImportService.Files;
using RMon.ValuesExportImportService.Processing.Common;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Processing.Parse
{
    class ParseMatrix24X31Logic
    {
        private readonly IMatrixReader _matrixReader;
        private readonly DbValuesAnalyzer _dbValuesAnalyzer;

        public ParseMatrix24X31Logic(DbValuesAnalyzer dbValuesAnalyzer, Matrix24X31Reader matrixReader)
        {
            _matrixReader = matrixReader;
            _dbValuesAnalyzer = dbValuesAnalyzer;
        }

        public async Task<List<ValueInfo>> AnalyzeAsync(IList<LocalFile> files, Matrix24X31ParsingParameters taskParams, ParseProcessingContext context, CancellationToken ct)
        {
            ValidateParameters(taskParams);
            var logicDevicePropertyValueCell = ExcelCellAddressConverter.CellAddressConvert(taskParams.LogicDevicePropertyCell);
            var cellStart = ExcelCellAddressConverter.CellAddressConvert(taskParams.FirstValueCell);
            var dateColumnNumber = ExcelCellAddressConverter.ColNumberConvert(taskParams.DateColumn);
            var timeRowNumber = int.Parse(taskParams.TimeRow);

            var excelResults = new List<ExcelResult>();
            foreach (var file in files)
            {
                await context.LogInfo(TextParse.ReadingFile.With(file.Path, ValuesParseFileFormatType.Matrix24X31.ToString())).ConfigureAwait(false);

                var message = _matrixReader.ReadExcelBook(file.Body, logicDevicePropertyValueCell, cellStart, dateColumnNumber, timeRowNumber, context);
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
        private void ValidateParameters(Matrix24X31ParsingParameters taskParams)
        {
            if (string.IsNullOrEmpty(taskParams.LogicDevicePropertyCode))
                throw new TaskException(TextParse.MissingLogicDevicePropertyCode);
            if (string.IsNullOrEmpty(taskParams.LogicDevicePropertyCell))
                throw new TaskException(TextParse.MissingLogicDevicePropertyCellAddress);
            if (string.IsNullOrEmpty(taskParams.TagCode))
                throw new TaskException(TextParse.MissingTagCode);
            if (string.IsNullOrEmpty(taskParams.FirstValueCell))
                throw new TaskException(TextParse.MissingFirstValueCellAddress);
            if (string.IsNullOrEmpty(taskParams.DateColumn))
                throw new TaskException(TextParse.MissingDateColumnNumber);
            if (string.IsNullOrEmpty(taskParams.TimeRow))
                throw new TaskException(TextParse.MissingTimeRowNumber);
            if (!int.TryParse(taskParams.TimeRow, out _))
                throw new TaskException(TextParse.IncorrectTimeRowNumber);
        }
    }
}
