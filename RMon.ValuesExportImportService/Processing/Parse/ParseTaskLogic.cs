using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RMon.Configuration.Options;
using RMon.Core.Base;
using RMon.Core.Files;
using RMon.Data.Provider;
using RMon.Data.Provider.Values;
using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesParseTaskDto;
using RMon.Globalization;
using RMon.Globalization.String;
using RMon.Values.ExportImport.Core;
using RMon.ValuesExportImportService.Configuration;
using RMon.ValuesExportImportService.Extensions;
using RMon.ValuesExportImportService.Files;
using RMon.ValuesExportImportService.Processing.Common;
using RMon.ValuesExportImportService.Processing.Extensions;
using RMon.ValuesExportImportService.ServiceBus;
using RMon.ValuesExportImportService.Text;
using Task = System.Threading.Tasks.Task;

namespace RMon.ValuesExportImportService.Processing.Parse
{
    class ParseTaskLogic : IParseTaskLogic
    {
        private readonly IOptionsMonitor<Service> _serviceOptions;
        private readonly IOptionsMonitor<ValuesParseOptions> _valuesParseOptions;
        private readonly IFileStorage _fileStorage;
        private readonly ISimpleFactory<IValueRepository> _valueRepositorySimpleFactory;
        private readonly IParseTaskLogger _taskLogger;
        private readonly ParseXml80020Logic _parseXml80020Logic;
        private readonly ParseMatrix24X31Logic _parseMatrix24X31Logic;
        private readonly ParseMatrix31X24Logic _parseMatrix31X24Logic;
        private readonly ParseTableLogic _parseTableLogic;
        private readonly ParseFlexibleFormatLogic _parseFlexibleFormatLogic;
        private readonly ITransformationRatioCalculator _transformationRatioCalculator;
        private readonly IValuesLogger _valuesLogger;

        /// <summary>
        /// Конструктор 1
        /// </summary>
        /// <param name="serviceOptions">Опции сервиса</param>
        /// <param name="valuesParseOptions">Опциии парсинга</param>
        /// <param name="valueRepositorySimpleFactory">Фабрика для создания репозитория значений</param>
        /// <param name="taskLogger">Логгер для заданий</param>
        /// <param name="fileStorage">Файловое хранилище</param>
        /// <param name="parseXml80020Logic">Логика для парсинга формата 80020</param>
        /// <param name="parseMatrix24X31Logic">Логика для парсинга матрицы 24x31</param>
        /// <param name="parseMatrix31X24Logic">Логика для парсинга матрицы 31x24</param>
        /// <param name="parseTableLogic">Логика для парсинга "Таблицы"</param>
        /// <param name="parseFlexibleFormatLogic">Логика для парсинга гибкого формата</param>
        /// <param name="transformationRatioCalculator">Калькулятор коэффициентов трансформации</param>
        /// <param name="valuesLogger">Логгер для полученных значений</param>
        public ParseTaskLogic(IOptionsMonitor<Service> serviceOptions,
            IOptionsMonitor<ValuesParseOptions> valuesParseOptions,
            ISimpleFactory<IValueRepository> valueRepositorySimpleFactory,
            IParseTaskLogger taskLogger,
            IFileStorage fileStorage,
            ParseXml80020Logic parseXml80020Logic,
            ParseMatrix24X31Logic parseMatrix24X31Logic,
            ParseMatrix31X24Logic parseMatrix31X24Logic,
            ParseTableLogic parseTableLogic,
            ParseFlexibleFormatLogic parseFlexibleFormatLogic,
            ITransformationRatioCalculator transformationRatioCalculator,
            IValuesLogger valuesLogger)
        {
            _serviceOptions = serviceOptions;
            _valuesParseOptions = valuesParseOptions;
            _valueRepositorySimpleFactory = valueRepositorySimpleFactory;
            _taskLogger = taskLogger;
            _fileStorage = fileStorage;
            _parseXml80020Logic = parseXml80020Logic;
            _parseMatrix24X31Logic = parseMatrix24X31Logic;
            _parseMatrix31X24Logic = parseMatrix31X24Logic;
            _parseTableLogic = parseTableLogic;
            _parseFlexibleFormatLogic = parseFlexibleFormatLogic;
            _transformationRatioCalculator = transformationRatioCalculator;
            _valuesLogger = valuesLogger;
        }

        
        /// <inheritdoc/>
        public async Task StartTaskAsync(ITask receivedTask, CancellationToken ct = default)
        {
            if (receivedTask is IValuesParseTask task)
            {
                var instanceName = _serviceOptions.CurrentValue.InstanceName;
                var dbTask = task.ToDbTask(instanceName);
                var context = new ParseProcessingContext(task, dbTask, _taskLogger, task.IdUser.Value);

                try
                {
                    await context.LogStarted(TextTask.Start).ConfigureAwait(false);
                    await context.LogInfo(TextTask.ValidateParameters).ConfigureAwait(false);
                    ValidateParameters(task);
                    await ValidateReceiveFilesSizeLimitAsync(task.Parameters.Files, ct).ConfigureAwait(false);

                    await context.LogInfo(TextParse.LoadingFiles, 10).ConfigureAwait(false);
                    var files = await ReceiveFilesAsync(task.Parameters.Files, ct).ConfigureAwait(false);

                    var values = task.Parameters.FileFormatType switch
                    {
                        ValuesParseFileFormatType.Xml80020 => await _parseXml80020Logic.AnalyzeAsync(files, task.Parameters.Xml80020Parameters, context, ct).ConfigureAwait(false),
                        ValuesParseFileFormatType.Matrix24X31 => await _parseMatrix24X31Logic.AnalyzeAsync(files, task.Parameters.Matrix24X31Parameters, context, ct).ConfigureAwait(false),
                        ValuesParseFileFormatType.Matrix31X24 => await _parseMatrix31X24Logic.AnalyzeAsync(files, task.Parameters.Matrix31X24Parameters, context, ct).ConfigureAwait(false),
                        ValuesParseFileFormatType.Table => await _parseTableLogic.AnalyzeAsync(files, task.Parameters.TableParameters, context, ct).ConfigureAwait(false),
                        ValuesParseFileFormatType.Flexible => await _parseFlexibleFormatLogic.AnalyzeAsync(files, context, ct).ConfigureAwait(false),
                        _ => throw new ArgumentOutOfRangeException(),
                    };

                    if (!values.Any())
                        throw new TaskException(TextParse.MissingParseValuesError);

                    if (task.Parameters.UseTransformationRatio)
                    {
                        await context.LogInfo(TextParse.UseTransformationRatio, 60).ConfigureAwait(false);
                        await _transformationRatioCalculator.LoadTagsRatioFromDbAsync(values.Select(t => t.IdTag).ToList(), ct).ConfigureAwait(false);

                        foreach (var value in values)
                        {
                            var tag = _transformationRatioCalculator.TagsRatio.SingleOrDefault(t => t.IdTag == value.IdTag);
                            if (tag != null)
                                value.Value.ValueFloat = value.Value.ValueFloat * tag.TransformationRatio * tag.Ratio + tag.Offset;
                        }
                    }
                    
                    await context.LogInfo(TextParse.LoadingCurrentValues, 80).ConfigureAwait(false);
                    await LoadCurrentValuesFromDb(context, values).ConfigureAwait(false);

                    _valuesLogger.LogSendValues(task.CorrelationId, values);
                    await context.LogFinished(TextTask.FinishSuccess, values).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    await context.LogAborted(TextTask.FinishAborted).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    await context.LogAborted(TextTask.FinishAborted).ConfigureAwait(false);
                }
                catch (UserFormattedException ex)
                {
                    await context.LogFailed(TextTask.FinishFailed.With(ex.FormattedMessage), ex).ConfigureAwait(false);
                }
                catch (DataProviderException ex)
                {
                    await context.LogFailed(TextTask.FinishFailed.With(ex.String), ex).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await context.LogFailed(TextTask.FinishFailed.With(new I18nString("", ex.Message)), ex).ConfigureAwait(false);
                }
            }
        }

        public void AbortTask(ITask receivedTask, StateMachineInstance instance) => instance.CancellationTokenSource.Cancel();


        /// <summary>
        /// Проверяет корректность полученных параметров
        /// </summary>
        /// <param name="task"></param>
        private static void ValidateParameters(IValuesParseTask task)
        {
            if (task?.Parameters.Files == null || !task.Parameters.Files.Any())
                throw new TaskException(TextParse.NoFilesError);
            if (task.IdUser == null)
                throw new TaskException(TextParse.NoUserIdError);
        }

        /// <summary>
        /// Асинхронно проверят размер всех получаемых файлов и в случае превышения заданного в настройках лимита генерирует исключение <see cref="TaskException"/>
        /// </summary>
        /// <param name="files">Список получаемых файлов</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns></returns>
        private async Task ValidateReceiveFilesSizeLimitAsync(IList<FileInStorage> files, CancellationToken cancellationToken)
        {
            var tasks = files.Select(file => _fileStorage.GetFileInfoAsync(file.Path, cancellationToken)).ToList();
            await Task.WhenAll(tasks).ConfigureAwait(false);
            var size = tasks.Sum(t => t.Result.Size);
            if (size > _valuesParseOptions.CurrentValue.TotalParseFilesSize)
                throw new TaskException(TextParse.FilesSizeExceedLimit.With((int)Math.Round(_valuesParseOptions.CurrentValue.TotalParseFilesSize / 1024d)));
        }

        /// <summary>
        /// Получает файлы <see cref="files"/> из файлового хранилища
        /// </summary>
        /// <param name="files">Список получаемых файлов</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Список файлов</returns>
        private async Task<IList<LocalFile>> ReceiveFilesAsync(IList<FileInStorage> files, CancellationToken cancellationToken = default)
        {
            var storedFiles = new List<LocalFile>();
            var tasks = files.Select(async file =>
            {
                var fileBody = await _fileStorage.GetFileAsync(file.Path, cancellationToken).ConfigureAwait(false);
                storedFiles.Add(new LocalFile(Path.GetFileName(file.Path), fileBody));
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
                        value.CurrentValue = dbValue.ToValueUnion();
                }
            }
        }
        

    }
}
