using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RMon.ValuesExportImportService.Excel;
using RMon.ValuesExportImportService.Excel.Flexible;
using RMon.ValuesExportImportService.Files;
using RMon.ValuesExportImportService.Processing.Parse;
using RMon.ValuesExportImportService.Processing.Permission;
using Task = System.Threading.Tasks.Task;

namespace RMon.ValuesExportImportService.Tests.ParseFlexible
{
    [TestClass]
    public class UnitTest
    {
        private readonly IExcelWorker _excelWorker;
        private readonly IPermissionLogic _permissionLogic;

        public UnitTest()
        {
            var logger = new Logger<ExcelWorker>(LoggerFactory.Create(c => c.AddDebug()));
            _excelWorker = new ExcelWorker(logger);
            _permissionLogic = new PermissionLogicStub();

        }

        [TestMethod]
        public async Task Test1()
        {
            var idLogicDevices = new List<long> {57122, 57123, 57172, 57174, 25, 29, 37};
            var logicDeviceRepository = new LogicDeviceRepositoryStub(idLogicDevices);
            var dataRepository = new DataRepositoryStub();

            var fileName = @"ParseFlexible\Files\Выгрузка значений 20210622 001252.xlsx";
            var fileBody = await File.ReadAllBytesAsync(fileName).ConfigureAwait(false);
            var idUser = 51;

            var taskLogger = new ParseTaskLoggerStub();
            var context = new ParseProcessingContext(null, null, taskLogger, idUser);

            var logic = new ParseFlexibleFormatLogic(logicDeviceRepository, dataRepository, _permissionLogic, _excelWorker);
            try
            {
                var values = await logic.AnalyzeAsync(new List<LocalFile> {new(fileName, fileBody)}, context, CancellationToken.None).ConfigureAwait(false);

                Assert.AreEqual(78326, values.Count, "Количество считанных значений не верно");

                Assert.AreEqual(109, values.Select(t => t.IdTag).Distinct().Count(), "Количество тегов не совпадает");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
