using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RMon.Values.ExportImport.Core;
using RMon.ValuesExportImportService.Common;
using RMon.ValuesExportImportService.Data;
using RMon.ValuesExportImportService.Excel.Matrix;
using RMon.ValuesExportImportService.Extensions;
using RMon.ValuesExportImportService.Processing.Common;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Processing.Parse
{
    class MatrixLogicBase
    {
        private readonly IDataRepository _dataRepository;

        public MatrixLogicBase(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        protected async Task<List<ValueInfo>> Analyze(List<(string FileName, IList<MatrixResult>)> messages, string logicDevicePropertyCode, string tagCode, ParseProcessingContext context, CancellationToken ct)
        {
            var result = new List<ValueInfo>();
            foreach (var message in messages)
            foreach (var matrixResult in message.Item2)
                try
                {
                    await context.LogInfo(TextParse.AnalyzeInfoFromExcelFile.With(message.FileName, matrixResult.SheetName)).ConfigureAwait(false);
                    var logicDevice = await _dataRepository.GetLogicDeviceByPropertyValueAsync(logicDevicePropertyCode, matrixResult.LogicDevicePropertyValue, ct).ConfigureAwait(false);
                    var tag = logicDevice.Tags.SingleOrDefault(t => t.LogicTagLink.LogicTagType.Code == tagCode);
                    if (tag == null)
                        throw new TaskException(TextParse.SelectedNoOneTagsError.With(logicDevice.Id, tagCode));

                    foreach (var value in matrixResult.Values)
                        result.Add(new ValueInfo(tag.Id, value.TimeStamp, value.Value));
                }
                catch (Exception e)
                {
                    await context.LogWarning(e.ConcatExceptionMessage(TextParse.SheetParseError.With(message.FileName, matrixResult.SheetName))).ConfigureAwait(false);
                }

            return result;
        }
    }
}
