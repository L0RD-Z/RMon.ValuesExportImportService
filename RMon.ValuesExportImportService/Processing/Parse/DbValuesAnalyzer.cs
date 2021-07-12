using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RMon.Values.ExportImport.Core;
using RMon.ValuesExportImportService.Data;
using RMon.ValuesExportImportService.Excel.Common;
using RMon.ValuesExportImportService.Extensions;
using RMon.ValuesExportImportService.Processing.Common;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Processing.Parse
{
    class DbValuesAnalyzer
    {
        private readonly IDataRepository _dataRepository;

        public DbValuesAnalyzer(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public async Task<List<ValueInfo>> Analyze(List<ExcelResult> excelResults, string logicDevicePropertyCode, string tagCode, ParseProcessingContext context, CancellationToken ct)
        {
            var result = new List<ValueInfo>();
            foreach (var message in excelResults)
            {
                var sheets = message.LogicDeviceValues.GroupBy(t => t.SheetName);
                foreach (var excelSheet in sheets)
                {
                    await context.LogInfo(TextParse.AnalyzeInfoFromExcelFile.With(message.FileName, excelSheet.Key)).ConfigureAwait(false);
                    foreach (var excelLogicDevice in excelSheet)
                        try
                        {
                            var logicDevice = await _dataRepository.GetLogicDeviceByPropertyValueAsync(logicDevicePropertyCode, excelLogicDevice.LogicDevicePropertyValue, ct).ConfigureAwait(false);
                            var tag = logicDevice.Tags.SingleOrDefault(t => t.LogicTagLink.LogicTagType.Code == tagCode);
                            if (tag == null)
                                throw new TaskException(TextParse.SelectedNoOneTagsError.With(logicDevice.Id, tagCode));

                            foreach (var value in excelLogicDevice.Values)
                                result.Add(new ValueInfo(tag.Id, value.TimeStamp, value.Value));
                        }
                        catch (Exception e)
                        {
                            await context.LogWarning(e.ConcatExceptionMessage(TextParse.SheetParseError.With(message.FileName, excelLogicDevice.SheetName))).ConfigureAwait(false);
                        }
                }
            }
            return result;
        }
    }
}
