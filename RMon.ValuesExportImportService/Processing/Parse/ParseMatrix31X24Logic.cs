using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RMon.Values.ExportImport.Core;
using RMon.Values.ExportImport.Core.FileFormatParameters;
using RMon.ValuesExportImportService.Excel.Matrix;
using RMon.ValuesExportImportService.Extensions;
using RMon.ValuesExportImportService.Files;
using RMon.ValuesExportImportService.Processing.Common;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Processing.Parse
{
    class ParseMatrix31X24Logic
    {
        private readonly IMatrixReader _matrixReader;

        public ParseMatrix31X24Logic(Matrix31X24Reader matrixReader)
        {
            _matrixReader = matrixReader;
        }

        public async Task<List<ValueInfo>> AnalyzeAsync(IList<LocalFile> files, Matrix31X24ParsingParameters taskParams, ParseProcessingContext context, CancellationToken ct)
        {
            ValidateParameters(taskParams);
            var logicDevicePropertyValueCell = ExcelCellAddressConverter.CellAddressConvert(taskParams.LogicDevicePropertyCell);
            var cellStart = ExcelCellAddressConverter.CellAddressConvert(taskParams.FirstValueCell);
            var dateRowNumber = int.Parse(taskParams.DateRow);
            var timeColumnNumber = ExcelCellAddressConverter.ColNumberConvert(taskParams.TimeColumn);

            var messages = new List<(string FileName, IList<MatrixResult>)>();
            foreach (var file in files)
            {
                await context.LogInfo(TextParse.ReadingFile.With(file.Path, ValuesParseFileFormatType.Matrix24X31.ToString())).ConfigureAwait(false);

                var message = _matrixReader.ReadExcelBook(file.Body, logicDevicePropertyValueCell, cellStart, dateRowNumber, timeColumnNumber, context);
                messages.Add((file.Path, message));
            }

            var result = new List<ValueInfo>();



            return result;
        }

        /// <summary>
        /// Выполняет валидацию полученных параметров задания
        /// </summary>
        /// <param name="taskParams"></param>
        private void ValidateParameters(Matrix31X24ParsingParameters taskParams)
        {
            if (string.IsNullOrEmpty(taskParams.LogicDevicePropertyCode))
                throw new TaskException(TextParse.MissingLogicDevicePropertyCode);
            if (string.IsNullOrEmpty(taskParams.LogicDevicePropertyCell))
                throw new TaskException(TextParse.MissingLogicDevicePropertyCellAddress);
            if (string.IsNullOrEmpty(taskParams.TagCode))
                throw new TaskException(TextParse.MissingTagCode);
            if (string.IsNullOrEmpty(taskParams.FirstValueCell))
                throw new TaskException(TextParse.MissingFirstValueCellAddress);
            if (string.IsNullOrEmpty(taskParams.DateRow))
                throw new TaskException(TextParse.MissingDateRowNumber);
            if (!int.TryParse(taskParams.DateRow, out _))
                throw new TaskException(TextParse.IncorrectDateRowNumber);
            if (string.IsNullOrEmpty(taskParams.TimeColumn))
                throw new TaskException(TextParse.MissingTimeColumnNumber);
            
        }
    }
}
