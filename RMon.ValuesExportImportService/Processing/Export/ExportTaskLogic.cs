using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RMon.Configuration.Options;
using RMon.Core.Files;
using RMon.Data.Provider;
using RMon.Data.Provider.Esb.Entities.ValuesExportImport;
using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesExportTaskDto;
using RMon.Globalization;
using RMon.Globalization.String;
using RMon.ValuesExportImportService.Data;
using RMon.ValuesExportImportService.Excel;
using RMon.ValuesExportImportService.Files;
using RMon.ValuesExportImportService.Globalization;
using RMon.ValuesExportImportService.Processing.Common;
using RMon.ValuesExportImportService.ServiceBus;
using RMon.ValuesExportImportService.Text;
using RMon.ValuesExportImportService.Common;
using RMon.ValuesExportImportService.Extensions;
using RMon.ValuesExportImportService.Processing.Permission;

namespace RMon.ValuesExportImportService.Processing.Export
{
    class ExportTaskLogic : BaseTaskLogic, IExportTaskLogic
    {
        private readonly ExportTaskLogger _taskLogger;
        private readonly IEntityReader _entityReader;
        private readonly IExcelWorker _excelWorker;


        /// <summary>
        /// Конструктор 1
        /// </summary>
        /// <param name="logger">Логгер</param>
        /// <param name="serviceOptions">Опции сервиса</param>
        /// <param name="taskFactoryRepositoryConfigurator">Конфигуратор репозиторияя для логирования хода выполнения задач</param>
        /// <param name="dataRepository">Репозиторий данных</param>
        /// <param name="taskLogger">Логгер для заданий</param>
        /// <param name="permissionLogic">Логика работы с прадвами доступа</param>
        /// <param name="fileStorage">Файловое хранилище</param>
        /// <param name="excelWorker">Работник с Excel</param>
        /// <param name="entityReader">Логика загрузки сущностей из БД</param>
        /// <param name="globalizationProviderFactory"></param>
        /// <param name="languageRepository"></param>
        public ExportTaskLogic(
            ILogger<ExportTaskLogic> logger,
            IOptionsMonitor<Service> serviceOptions,
            IRepositoryFactoryConfigurator taskFactoryRepositoryConfigurator,
            IDataRepository dataRepository, 
            ExportTaskLogger taskLogger,
            IPermissionLogic permissionLogic,
            IFileStorage fileStorage,
            IExcelWorker excelWorker,
            IEntityReader entityReader,
            IGlobalizationProviderFactory globalizationProviderFactory,
            ILanguageRepository languageRepository)
            : base(logger, serviceOptions, taskFactoryRepositoryConfigurator, dataRepository, permissionLogic, fileStorage, globalizationProviderFactory, languageRepository)
        {
            _taskLogger = taskLogger;
            _entityReader = entityReader;
            _excelWorker = excelWorker;
        }

        
        public override async Task StartTaskAsync(ITask receivedTask, CancellationToken ct = default)
        {
            if (receivedTask is IValuesExportTask task)
            {
                var instanceName = ServiceOptions.CurrentValue.InstanceName;
                var dbTask = task.ToDbTask(instanceName);
                var context = CreateProcessingContext(task, dbTask);

                try
                {
                    await context.LogStarted(TextExport.Start).ConfigureAwait(false);
                    await context.LogInfo(TextExport.ValidateParameters).ConfigureAwait(false);
                    ValidateParameters(task);

                    await context.LogInfo(TextExport.LoadingData, 10).ConfigureAwait(false);

                    var exportTable = await _entityReader.Read(task.Parameters, task.IdUser.Value, ct).ConfigureAwait(false);

                    await context.LogInfo(TextExport.BuildingExcel, 60).ConfigureAwait(false);
                    var fileBody = _excelWorker.WriteWorksheet(exportTable, context.GlobalizationProvider);
                    
                    await context.LogInfo(TextExport.StoringFile, 90).ConfigureAwait(false);
                    var currentDate = await DataRepository.GetDateAsync().ConfigureAwait(false);
                    var filePath = FilePathCreate(Guid.NewGuid(), currentDate, context);
                    await FileStorage.StoreFileAsync(filePath, fileBody, ct).ConfigureAwait(false);

                    await context.LogFinished(TextExport.FinishSuccess, new List<FileInStorage> { new(filePath, fileBody.Length) }).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    await context.LogAborted(TextExport.FinishAborted).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    await context.LogAborted(TextExport.FinishAborted).ConfigureAwait(false);
                }
                catch (UserFormattedException ex)
                {
                    await context.LogFailed(TextExport.FinishFailed.With(ex.FormattedMessage), ex).ConfigureAwait(false);
                }
                catch (DataProviderException ex)
                {
                    await context.LogFailed(TextExport.FinishFailed.With(ex.String), ex).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await context.LogFailed(TextExport.FinishFailed.With(new I18nString("", ex.Message)), ex).ConfigureAwait(false);
                }
            }
        }

        public override void AbortTask(ITask receivedTask, StateMachineInstance instance) => instance.CancellationTokenSource.Cancel();


        /// <summary>
        /// Проверяет корректность полученных параметров
        /// </summary>
        /// <param name="task"></param>
        private void ValidateParameters(IValuesExportTask task)
        {
            if (task.IdUser == null)
                throw new TaskException(TextExport.NoUserIdError);
            if (!task.Parameters.IdLogicDevices.Any())
                throw new TaskException(TextExport.NoLogicDevicesError);
            if (!task.Parameters.TagTypeCodes.Any())
                throw new TaskException(TextExport.NoTagCodesError);
        }

        /// <summary>
        /// Генерирует имя файла
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <param name="date">Текущая дата</param>
        /// <param name="context"></param>
        /// <returns></returns>
        private string FilePathCreate(Guid guid, DateTime date, IProcessingContext context)
        {
            var fileName = TextExport.ExportFileName.With(date).ToString(context.GlobalizationProvider);
            return $@"{guid}\{fileName.Remove(Path.GetInvalidFileNameChars())}";
        }

        private ExportProcessingContext CreateProcessingContext(IValuesExportTask task, DbValuesExportImportTask dbTask)
        {
            var processingContext = new ExportProcessingContext(task, dbTask, _taskLogger, task.IdUser.Value);

            var idLanguage = LanguageRepository.GetUserLanguage(task.IdUser.Value).Result;
            if (idLanguage != null)
                processingContext.GlobalizationProvider = GlobalizationProviderFactory.GetGlobalizationProvider(idLanguage.Value);
            

            return processingContext;
        }
    }
}
