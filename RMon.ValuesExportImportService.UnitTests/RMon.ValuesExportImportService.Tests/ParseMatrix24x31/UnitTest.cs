using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RMon.ValuesExportImportService.Excel.Matrix;
using RMon.ValuesExportImportService.Files;
using RMon.ValuesExportImportService.Processing.Parse;
using System.IO;
using System.Linq;
using System.Threading;
using RMon.Values.ExportImport.Core.FileFormatParameters;

namespace RMon.ValuesExportImportService.Tests.ParseMatrix24x31
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public async Task Test1()
        {
            var dataRepository = new DataRepositoryStub();
            var matrixReader = new Matrix24X31Reader();
            var logic = new ParseMatrix24X31Logic(dataRepository, matrixReader);

            var fileName = @"ParseMatrix24x31\Files\24x31.xls";
            var fileBody = await File.ReadAllBytesAsync(fileName).ConfigureAwait(false);

            var taskParams = new Matrix24X31ParsingParameters()
            {
                LogicDevicePropertyCode = "AgrNo",
                LogicDevicePropertyCell = "H3",
                TagCode = "dHHA+",
                FirstValueCell = "C14",
                DateColumn = "A",
                TimeRow = "13",
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
            var matrixReader = new Matrix24X31Reader();
            var logic = new ParseMatrix24X31Logic(dataRepository, matrixReader);

            var fileName = @"ParseMatrix24x31\Files\24x31 (31 день).xls";
            var fileBody = await File.ReadAllBytesAsync(fileName).ConfigureAwait(false);

            var taskParams = new Matrix24X31ParsingParameters()
            {
                LogicDevicePropertyCode = "AgrNo",
                LogicDevicePropertyCell = "H3",
                TagCode = "dHHA+",
                FirstValueCell = "C14",
                DateColumn = "A",
                TimeRow = "13",
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
