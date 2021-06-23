using System;
using System.Collections.Generic;
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
using RMon.ESB.Core.ValuesParseTaskDto;
using RMon.Globalization.String;
using RMon.Values.ExportImport.Core;
using RMon.ValuesExportImportService.Data;
using RMon.ValuesExportImportService.Exceptions;
using RMon.ValuesExportImportService.Extensions;
using RMon.ValuesExportImportService.Files;
using RMon.ValuesExportImportService.Globalization;
using RMon.ValuesExportImportService.Processing.Common;
using RMon.ValuesExportImportService.Processing.Export;
using RMon.ValuesExportImportService.Processing.Permission;
using RMon.ValuesExportImportService.ServiceBus;
using RMon.ValuesExportImportService.Text;
using Task = System.Threading.Tasks.Task;

namespace RMon.ValuesExportImportService.Processing.Parse
{
    class ParseTaskLogic : BaseTaskLogic, IParseTaskLogic
    {
        private readonly ParseTaskLogger _taskLogger;
        private readonly Parse80020Logic _parse80020Logic;

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
        /// <param name="parse80020Logic">Логика для парсинга формата 80020</param>
        /// <param name="globalizationProviderFactory"></param>
        /// <param name="languageRepository"></param>
        public ParseTaskLogic(
            ILogger<ParseTaskLogic> logger,
            IOptionsMonitor<Service> serviceOptions,
            IRepositoryFactoryConfigurator taskFactoryRepositoryConfigurator,
            IDataRepository dataRepository,
            ParseTaskLogger taskLogger,
            IPermissionLogic permissionLogic,
            IFileStorage fileStorage,
            Parse80020Logic parse80020Logic,
            IGlobalizationProviderFactory globalizationProviderFactory,
            ILanguageRepository languageRepository)
            : base(logger, serviceOptions,  taskFactoryRepositoryConfigurator, dataRepository, permissionLogic, fileStorage, globalizationProviderFactory, languageRepository)
        {
            _taskLogger = taskLogger;
            _parse80020Logic = parse80020Logic;
        }

        
        /// <inheritdoc/>
        public override async Task StartTaskAsync(ITask receivedTask, CancellationToken ct = default)
        {
            if (receivedTask is IValuesParseTask task)
            {
                var instanceName = ServiceOptions.CurrentValue.InstanceName;
                var dbTask = task.ToDbTask(instanceName);
                var context = CreateProcessingContext(task, dbTask);

                try
                {
                    await context.LogStarted(TextParse.Start).ConfigureAwait(false);
                    await context.LogInfo(TextParse.ValidateParameters).ConfigureAwait(false);
                    ValidateParameters(task);

                    await context.LogInfo(TextParse.LoadingFiles, 10).ConfigureAwait(false);
                    var files = await ReceiveFilesAsync(task.Parameters.Files, ct).ConfigureAwait(false);

                    var result = new List<ValueInfo>();
                    switch (task.Parameters.FileFormatType)
                    {
                        case ValuesParseFileFormatType.Xml80020:
                        {
                            result = await _parse80020Logic.ParseFormat80020Async(files, task.Parameters.Xml80020Parameters, context, ct).ConfigureAwait(false);
                        }
                            break;
                        case ValuesParseFileFormatType.Matrix24X31:
                            break;
                        case ValuesParseFileFormatType.Matrix31X24:
                            break;
                        case ValuesParseFileFormatType.Table:
                            break;
                        case ValuesParseFileFormatType.Flexible:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    await context.LogFinished(TextParse.FinishSuccess, result).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    await context.LogAborted(TextParse.FinishAborted).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    await context.LogAborted(TextParse.FinishAborted).ConfigureAwait(false);
                }
                catch (UserException ex)
                {
                    await context.LogFailed(TextExport.FinishFailed.With(ex.String), ex).ConfigureAwait(false);
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
        private void ValidateParameters(IValuesParseTask task)
        {
            if (task?.Parameters.Files == null || !task.Parameters.Files.Any())
                throw new TaskException(TextParse.NoFilesError);
            if (task.IdUser == null)
                throw new TaskException(TextParse.NoUserIdError);
        }


        /// <summary>
        /// Получает файлы <see cref="files"/> из файлового хранилища
        /// </summary>
        /// <param name="files">Список получаемых файлов</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Список файлов</returns>
        private async Task<IList<LocalFile>> ReceiveFilesAsync(IEnumerable<FileInStorage> files, CancellationToken cancellationToken = default)
        {
            var storedFiles = new List<LocalFile>();
            var tasks = files.Select(async file =>
            {
                var fileBody = await FileStorage.GetFileAsync(file.Path, cancellationToken).ConfigureAwait(false);
                storedFiles.Add(new LocalFile(file.Path, fileBody));
            });
            await Task.WhenAll(tasks).ConfigureAwait(false);
            return storedFiles;
        }

        private ParseProcessingContext CreateProcessingContext(IValuesParseTask task, DbValuesExportImportTask dbTask)
        {
            var processingContext = new ParseProcessingContext(task, dbTask, _taskLogger, task.IdUser.Value);

            return processingContext;
        }

        
    }
}
