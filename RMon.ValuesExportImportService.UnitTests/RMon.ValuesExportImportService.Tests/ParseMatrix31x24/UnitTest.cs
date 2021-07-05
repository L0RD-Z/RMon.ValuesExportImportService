using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RMon.Values.ExportImport.Core.FileFormatParameters;
using RMon.ValuesExportImportService.Excel.Matrix;
using RMon.ValuesExportImportService.Files;
using RMon.ValuesExportImportService.Processing.Parse;

namespace RMon.ValuesExportImportService.Tests.ParseMatrix31x24
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public async Task Test1()
        {
            var dataRepository = new DataRepositoryStub();
            var dbValuesAnalyzer = new DbValuesAnalyzer(dataRepository);
            var matrixReader = new Matrix31X24Reader();
            var logic = new ParseMatrix31X24Logic(dbValuesAnalyzer, matrixReader);

            var fileName = @"ParseMatrix31x24\Files\31x24.xls";
            var fileBody = await File.ReadAllBytesAsync(fileName).ConfigureAwait(false);

            var taskParams = new Matrix31X24ParsingParameters()
            {
                LogicDevicePropertyCode = "AgrNo",
                LogicDevicePropertyCell = "H3",
                TagCode = "dHHA+",
                FirstValueCell = "B15",
                DateRow = "13",
                TimeColumn = "A",
            };
            var idUser = 51;

            var taskLogger = new ParseTaskLoggerStub();
            var context = new ParseProcessingContext(null, null, taskLogger, idUser);

            var values = await logic.AnalyzeAsync(new List<LocalFile> {new(fileName, fileBody)}, taskParams, context, CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual(672, values.Count, "Количество полученных значений неверно");

            var timestamps = values.Select(t => t.TimeStamp).Distinct().ToList();
            Assert.AreEqual(672, timestamps.Count, "Количество полученных таймстампов значений неверно");
        }

        [TestMethod]
        public async Task Test2()
        {
            var dataRepository = new DataRepositoryStub();
            var matrixReader = new Matrix31X24Reader();
            var dbValuesAnalyzer = new DbValuesAnalyzer(dataRepository);
            var logic = new ParseMatrix31X24Logic(dbValuesAnalyzer, matrixReader);

            var fileName = @"ParseMatrix31x24\Files\31x24 (31 день).xls";
            var fileBody = await File.ReadAllBytesAsync(fileName).ConfigureAwait(false);

            var taskParams = new Matrix31X24ParsingParameters()
            {
                LogicDevicePropertyCode = "AgrNo",
                LogicDevicePropertyCell = "H3",
                TagCode = "dHHA+",
                FirstValueCell = "B15",
                DateRow = "13",
                TimeColumn = "A",
            };
            var idUser = 51;

            var taskLogger = new ParseTaskLoggerStub();
            var context = new ParseProcessingContext(null, null, taskLogger, idUser);

            var values = await logic.AnalyzeAsync(new List<LocalFile> { new(fileName, fileBody) }, taskParams, context, CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual(744, values.Count, "Количество полученных значений неверно");

            var timestamps = values.Select(t => t.TimeStamp).Distinct().ToList();
            Assert.AreEqual(744, timestamps.Count, "Количество полученных таймстампов значений неверно");
        }
    }
}
