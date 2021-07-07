using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RMon.Configuration.Options;
using RMon.Core.Base;
using RMon.Core.Files;
using RMon.Data.Provider;
using RMon.Data.Provider.Esb.Entities.ValuesExportImport;
using RMon.Data.Provider.Values;
using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesParseTaskDto;
using RMon.Globalization;
using RMon.Globalization.String;
using RMon.Values.ExportImport.Core;
using RMon.ValuesExportImportService.Data;
using RMon.ValuesExportImportService.Extensions;
using RMon.ValuesExportImportService.Files;
using RMon.ValuesExportImportService.Globalization;
using RMon.ValuesExportImportService.Processing.Common;
using RMon.ValuesExportImportService.Processing.Permission;
using RMon.ValuesExportImportService.ServiceBus;
using RMon.ValuesExportImportService.Text;
using Task = System.Threading.Tasks.Task;

namespace RMon.ValuesExportImportService.Processing.Parse
{
    class ParseTaskLogic : BaseTaskLogic, IParseTaskLogic
    {
        private readonly ISimpleFactory<IValueRepository> _valueRepositorySimpleFactory;
        private readonly IParseTaskLogger _taskLogger;
        private readonly ParseXml80020Logic _parseXml80020Logic;
        private readonly ParseMatrix24X31Logic _parseMatrix24X31Logic;
        private readonly ParseMatrix31X24Logic _parseMatrix31X24Logic;
        private readonly ParseFlexibleFormatLogic _parseFlexibleFormatLogic;

        /// <summary>
        /// Конструктор 1
        /// </summary>
        /// <param name="logger">Логгер</param>
        /// <param name="serviceOptions">Опции сервиса</param>
        /// <param name="valueRepositorySimpleFactory">Фабрика для создания репозитория значений</param>
        /// <param name="taskFactoryRepositoryConfigurator">Конфигуратор репозиторияя для логирования хода выполнения задач</param>
        /// <param name="dataRepository">Репозиторий данных</param>
        /// <param name="taskLogger">Логгер для заданий</param>
        /// <param name="permissionLogic">Логика работы с прадвами доступа</param>
        /// <param name="fileStorage">Файловое хранилище</param>
        /// <param name="parseXml80020Logic">Логика для парсинга формата 80020</param>
        /// <param name="parseMatrix24X31Logic">Логика для парсинга матрицы 24x31</param>
        /// <param name="parseMatrix31X24Logic">Логика для парсинга матрицы 31x24</param>
        /// <param name="parseFlexibleFormatLogic">Логика для парсинга гибкого формата</param>
        /// <param name="globalizationProviderFactory"></param>
        /// <param name="languageRepository"></param>
        public ParseTaskLogic(
            ILogger<ParseTaskLogic> logger,
            IOptionsMonitor<Service> serviceOptions,
            ISimpleFactory<IValueRepository> valueRepositorySimpleFactory,
            IRepositoryFactoryConfigurator taskFactoryRepositoryConfigurator,
            IDataRepository dataRepository,
            IParseTaskLogger taskLogger,
            IPermissionLogic permissionLogic,
            IFileStorage fileStorage,
            ParseXml80020Logic parseXml80020Logic,
            ParseMatrix24X31Logic parseMatrix24X31Logic,
            ParseMatrix31X24Logic parseMatrix31X24Logic,
            ParseFlexibleFormatLogic parseFlexibleFormatLogic,
            IGlobalizationProviderFactory globalizationProviderFactory,
            ILanguageRepository languageRepository)
            : base(logger, serviceOptions,  taskFactoryRepositoryConfigurator, dataRepository, permissionLogic, fileStorage, globalizationProviderFactory, languageRepository)
        {
            _valueRepositorySimpleFactory = valueRepositorySimpleFactory;
            _taskLogger = taskLogger;
            _parseXml80020Logic = parseXml80020Logic;
            _parseMatrix24X31Logic = parseMatrix24X31Logic;
            _parseMatrix31X24Logic = parseMatrix31X24Logic;
            _parseFlexibleFormatLogic = parseFlexibleFormatLogic;
        }

        
        /// <inheritdoc/>
        public override async Task StartTaskAsync(ITask receivedTask, CancellationToken ct = default)
        {
            if (receivedTask is IValuesParseTask task)
            {
                var instanceName = ServiceOptions.CurrentValue.InstanceName;
                var dbTask = task.ToDbTask(instanceName);
                var context = new ParseProcessingContext(task, dbTask, _taskLogger, task.IdUser.Value);

                try
                {
                    await context.LogStarted(TextParse.Start).ConfigureAwait(false);
                    await context.LogInfo(TextParse.ValidateParameters).ConfigureAwait(false);
                    ValidateParameters(task);

                    await context.LogInfo(TextParse.LoadingFiles, 10).ConfigureAwait(false);
                    var files = await ReceiveFilesAsync(task.Parameters.Files, ct).ConfigureAwait(false);

                    var values = new List<ValueInfo>();
                    switch (task.Parameters.FileFormatType)
                    {
                        case ValuesParseFileFormatType.Xml80020:
                            values = await _parseXml80020Logic.AnalyzeAsync(files, task.Parameters.Xml80020Parameters, context, ct).ConfigureAwait(false);
                            break;
                        case ValuesParseFileFormatType.Matrix24X31:
                            values = await _parseMatrix24X31Logic.AnalyzeAsync(files, task.Parameters.Matrix24X31Parameters, context, ct).ConfigureAwait(false);
                            break;
                        case ValuesParseFileFormatType.Matrix31X24:
                            values = await _parseMatrix31X24Logic.AnalyzeAsync(files, task.Parameters.Matrix31X24Parameters, context, ct).ConfigureAwait(false);
                            break;
                        case ValuesParseFileFormatType.Table:
                            break;
                        case ValuesParseFileFormatType.Flexible:
                            values = await _parseFlexibleFormatLogic.AnalyzeAsync(files, context, ct).ConfigureAwait(false);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    await context.LogInfo(TextParse.LoadingCurrentValues, 70).ConfigureAwait(false);
                    await LoadCurrentValuesFromDb(context, values).ConfigureAwait(false);

                    await context.LogFinished(TextParse.FinishSuccess, values).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    await context.LogAborted(TextParse.FinishAborted).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    await context.LogAborted(TextParse.FinishAborted).ConfigureAwait(false);
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

        /// <summary>
        /// Выполняет загрузку текущих значений из БД
        /// </summary>
        /// <param name="context"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private async Task LoadCurrentValuesFromDb(ParseProcessingContext context, List<ValueInfo> values)
        {
            var valuesRepository = _valueRepositorySimpleFactory.Create();

            var groupValues = values.GroupBy(t => t.IdTag);
            foreach (var groupValue in groupValues)
            {
                var timeRange = new TimeRange(groupValue.Min(t => t.TimeStamp), groupValue.Max(t => t.TimeStamp));
                await context.LogInfo(TextParse.LoadingCurrentValuesForTag.With(groupValue.Key, timeRange.DateStart.Value, timeRange.DateEnd.Value)).ConfigureAwait(false);
                var dbValues = await valuesRepository.GetValuesAsync(groupValue.Key, timeRange).ConfigureAwait(false);
                foreach (var value in groupValue)
                {
                    var dbValue = dbValues.SingleOrDefault(t => t.Datetime == value.TimeStamp);
                    if (dbValue != null)
                        value.CurrentValue = new ValueUnion("Normal", dbValue.ValueFloat);
                }
            }
        }

    }
}
