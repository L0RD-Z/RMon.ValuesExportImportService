using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RMon.Values.ExportImport.Core.FileFormatParameters;
using RMon.ValuesExportImportService.Excel.Table;
using RMon.ValuesExportImportService.Files;
using RMon.ValuesExportImportService.Processing.Parse;

namespace RMon.ValuesExportImportService.Tests.ParseTable
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public async Task Test1()
        {
            var dataRepository = new DataRepositoryStub();
            var dbValuesAnalyzer = new DbValuesAnalyzer(dataRepository);
            var excelReader = new TableReader();
            var logic = new ParseTableLogic(dbValuesAnalyzer, excelReader);

            var fileName = @"ParseTable\Files\Февраль 2021.xlsx";
            var fileBody = await File.ReadAllBytesAsync(fileName).ConfigureAwait(false);

            var taskParams = new TableParsingParameters()
            {
                LogicDevicePropertyCode = "AgrNo",
                LogicDevicePropertyRow = "2",
                TagCode = "dHHA+",
                FirstValueCell = "C3",
                DateColumn = "A",
                TimeColumn = "B",
            };
            var idUser = 51;

            var taskLogger = new ParseTaskLoggerStub();
            var context = new ParseProcessingContext(null, null, taskLogger, idUser);

            var values = await logic.AnalyzeAsync(new List<LocalFile> {new(fileName, fileBody)}, taskParams, context, CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual(4704, values.Count, "Количество полученных значений неверно");

            var timestamps = values.Select(t => t.TimeStamp).Distinct().ToList();
            Assert.AreEqual(672, timestamps.Count, "Количество полученных таймстампов значений неверно");
        }

        [TestMethod]
        public async Task Test2()
        {
            var dataRepository = new DataRepositoryStub();
            var dbValuesAnalyzer = new DbValuesAnalyzer(dataRepository);
            var excelReader = new TableReader();
            var logic = new ParseTableLogic(dbValuesAnalyzer, excelReader);

            var fileName = @"ParseTable\Files\Март 2021.xlsx";
            var fileBody = await File.ReadAllBytesAsync(fileName).ConfigureAwait(false);

            var taskParams = new TableParsingParameters()
            {
                LogicDevicePropertyCode = "AgrNo",
                LogicDevicePropertyRow = "2",
                TagCode = "dHHA+",
                FirstValueCell = "C3",
                DateColumn = "A",
                TimeColumn = "B",
            };
            var idUser = 51;

            var taskLogger = new ParseTaskLoggerStub();
            var context = new ParseProcessingContext(null, null, taskLogger, idUser);

            var values = await logic.AnalyzeAsync(new List<LocalFile> { new(fileName, fileBody) }, taskParams, context, CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual(5208, values.Count, "Количество полученных значений неверно");

            var timestamps = values.Select(t => t.TimeStamp).Distinct().ToList();
            Assert.AreEqual(744, timestamps.Count, "Количество полученных таймстампов значений неверно");
        }
    }
}
