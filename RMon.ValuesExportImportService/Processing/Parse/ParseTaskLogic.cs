using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RMon.Configuration.Options;
using RMon.Context.EntityStore;
using RMon.Core.Files;
using RMon.Data.Provider;
using RMon.Data.Provider.Esb.Entities.Commissioning;
using RMon.Data.Provider.Esb.Entities.ValuesExportImport;
using RMon.Data.Provider.Esb.ValuesExportImport.Parse;
using RMon.Data.Provider.Values;
using RMon.DriverCore;
using RMon.ESB.Core.CommissioningImportTaskDto;
using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesParseTaskDto;
using RMon.Globalization.String;
using RMon.Values.ExportImport.Core;
using RMon.Values.ExportImport.Core.FileFormatParameters;
using RMon.ValuesExportImportService.Data;
using RMon.ValuesExportImportService.Excel;
using RMon.ValuesExportImportService.Exceptions;
using RMon.ValuesExportImportService.Extensions;
using RMon.ValuesExportImportService.Files;
using RMon.ValuesExportImportService.Globalization;
using RMon.ValuesExportImportService.Processing.Common;
using RMon.ValuesExportImportService.Processing.Export;
using RMon.ValuesExportImportService.Processing.Parse.Format80020;
using RMon.ValuesExportImportService.Processing.Parse.Format80020.Entity;
using RMon.ValuesExportImportService.Processing.Permission;
using RMon.ValuesExportImportService.ServiceBus;
using RMon.ValuesExportImportService.Text;
using DateTime = System.DateTime;
using Message = RMon.ValuesExportImportService.Processing.Parse.Format80020.Entity.Message;
using Task = System.Threading.Tasks.Task;

namespace RMon.ValuesExportImportService.Processing.Parse
{
    class ParseTaskLogic : BaseTaskLogic, IParseTaskLogic
    {
        private readonly IOptionsMonitor<ValuesDatabase> _valuesDatabaseOptions;
        private readonly ParseTaskLogger _taskLogger;

        /// <summary>
        /// Конструктор 1
        /// </summary>
        /// <param name="logger">Логгер</param>
        /// <param name="serviceOptions">Опции сервиса</param>
        /// <param name="valuesDatabaseOptions"></param>
        /// <param name="taskFactoryRepositoryConfigurator">Конфигуратор репозиторияя для логирования хода выполнения задач</param>
        /// <param name="dataRepository">Репозиторий данных</param>
        /// <param name="taskLogger">Логгер для заданий</param>
        /// <param name="permissionLogic">Логика работы с прадвами доступа</param>
        /// <param name="fileStorage">Файловое хранилище</param>
        /// <param name="entityReader">Логика загрузки сущностей из БД</param>
        /// <param name="globalizationProviderFactory"></param>
        /// <param name="languageRepository"></param>
        public ParseTaskLogic(
            ILogger<ParseTaskLogic> logger,
            IOptionsMonitor<Service> serviceOptions,
            IOptionsMonitor<ValuesDatabase> valuesDatabaseOptions,
            IRepositoryFactoryConfigurator taskFactoryRepositoryConfigurator,
            IDataRepository dataRepository,
            ParseTaskLogger taskLogger,
            IPermissionLogic permissionLogic,
            IFileStorage fileStorage,
            IEntityReader entityReader,
            IGlobalizationProviderFactory globalizationProviderFactory,
            ILanguageRepository languageRepository)
            : base(logger, serviceOptions,  taskFactoryRepositoryConfigurator, dataRepository, permissionLogic, fileStorage, globalizationProviderFactory, languageRepository)
        {
            _valuesDatabaseOptions = valuesDatabaseOptions;
            _taskLogger = taskLogger;
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


                    switch (task.Parameters.FileFormatType)
                    {
                        case ValuesParseFileFormatType.Xml80020:
                        {
                            var result = await ParseXml80020(files, task.Parameters.Xml80020Parameters, context, ct).ConfigureAwait(false);
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

                    await context.LogFinished(TextParse.FinishSuccess, null).ConfigureAwait(false);
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

        private async Task<List<ValueInfo>> ParseXml80020(IList<LocalFile> files, Xml80020ParsingParameters taskParams, ParseProcessingContext context, CancellationToken ct)
        {
            var messages = new List<(string fileName, Message)>();
            foreach (var file in files)
            {
                await context.LogInfo(TextParse.ReadingFile.With(file.Path, ValuesParseFileFormatType.Xml80020.ToString())).ConfigureAwait(false);

                var message = Parser.Parse(file.Body);
                messages.Add((file.Path, message));
            }

            var result = new List<ValueInfo>();
            foreach (var message in messages)
            {
                var date = DateTime.ParseExact(message.Item2.DateTime.Day, "yyyyMMdd", CultureInfo.InvariantCulture);

                foreach (var area in message.Item2.Areas)
                {
                    if (taskParams.MeasuringPoint != null)
                    {
                        var values = await ParsePointsAsync(area.Name, area.MeasuringPoints, taskParams.MeasuringPoint, date, context, message.fileName, ct).ConfigureAwait(false);
                        result.AddRange(values);
                    }

                    if (taskParams.DeliveryPoint != null)
                    {
                        var values = await ParsePointsAsync(area.Name, area.DeliveryPoints, taskParams.DeliveryPoint, date, context, message.fileName, ct).ConfigureAwait(false);
                        result.AddRange(values);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Выполняет парсинг точек поставки (<see cref="DeliveryPoint"/>) / точек измерения (<see cref="MeasuringPoint"/>)
        /// </summary>
        /// <param name="areaName">Имя узла</param>
        /// <param name="points">Массив точек</param>
        /// <param name="pointParameters">Параметры задания</param>
        /// <param name="date">Дата файла</param>
        /// <param name="context">Контекст выполнения задания</param>
        /// <param name="fileName">Имя файла</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns></returns>
        private async Task<List<ValueInfo>> ParsePointsAsync(string areaName, MeasuringPoint[] points, Xml80020PointParameters pointParameters, DateTime date, ParseProcessingContext context, string fileName, CancellationToken ct)
        {
            var result = new List<ValueInfo>();
            if (points != null && points.Any())
                foreach (var measuringPoint in points)
                {
                    var logicDevice = await DataRepository.GetLogicDeviceByPropertyValueAsync(pointParameters.PointPropertyCode, measuringPoint.Code, ct).ConfigureAwait(false);
                    foreach (var measuringChannel in measuringPoint.MeasuringChannels)
                        result.AddRange(ParseChannel(measuringChannel, pointParameters, logicDevice, date, context));
                }
            else
                await context.LogWarning(TextParse.NotFoundSectionWarning.With(fileName, areaName, nameof(DeliveryPoint))).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Выполняет парсинг канала поставки
        /// </summary>
        /// <param name="measuringChannel">Канал</param>
        /// <param name="pointParameters">Параметры задания</param>
        /// <param name="logicDevice">Логическое устройство</param>
        /// <param name="date">Дата</param>
        /// <param name="processingContext">Контекст выполнения задания</param>
        /// <returns></returns>
        private List<ValueInfo> ParseChannel(MeasuringChannel measuringChannel, Xml80020PointParameters pointParameters, LogicDevice logicDevice, DateTime date, ParseProcessingContext processingContext)
        {
            var result = new List<ValueInfo>();

            var tagCode = GetTagCode(pointParameters, measuringChannel.Code);
            var tag = GetTag(logicDevice, tagCode);

            var timeStampTypeTag = TimeStampTypeEnum.GetTypeByValue(tag.DeviceTag.IdTimeStampType);

            ValueInfo lastValue = null;
            foreach (var period in measuringChannel.Periods)
            {
                var startTime = DateTime.ParseExact(period.Start, "HHmm", CultureInfo.InvariantCulture);
                var endTime = DateTime.ParseExact(period.End, "HHmm", CultureInfo.InvariantCulture);
                var value = double.Parse(period.Value.Val.Replace(",", "."), CultureInfo.InvariantCulture);

                var timeStampTypeValue = GetTimeStamp(startTime, endTime);

                if (timeStampTypeTag == timeStampTypeValue)
                    result.Add(ValueInfoCreate(tag.Id, date, endTime, value));
                else 
                    if (timeStampTypeTag == TimeStampTypeEnum.HalfHour && timeStampTypeValue == TimeStampTypeEnum.Hour)
                    {
                        result.Add(ValueInfoCreate(tag.Id, date, endTime.AddMinutes(-30), value / 2));
                        result.Add(ValueInfoCreate(tag.Id, date, endTime, value / 2));
                    }
                    else 
                        if (timeStampTypeTag == TimeStampTypeEnum.Hour && timeStampTypeValue == TimeStampTypeEnum.HalfHour)
                        {
                            if (endTime.Minute == 30)
                                lastValue = ValueInfoCreate(tag.Id, date, endTime, value);
                            else
                            {
                                if (lastValue == null)
                                    processingContext.LogWarning(TextParse.MissingValueWarning.With(tag.Id, startTime, endTime));
                                else
                                {
                                    var currentValue = ValueInfoCreate(tag.Id, date, endTime, value + lastValue.Value.ValueFloat.Value);
                                    if ((currentValue.TimeStamp - lastValue.TimeStamp).TotalMinutes == 60)
                                    {
                                        result.Add(currentValue);
                                        lastValue = null;
                                    }
                                    else //Если в периоде наткнулись на пропуск
                                    {
                                        lastValue = null;
                                        if (endTime.Minute == 30)
                                            lastValue = ValueInfoCreate(tag.Id, date, endTime, value);
                                        else
                                            processingContext.LogWarning(TextParse.MissingValueWarning.With(tag.Id, startTime, endTime));
                                    }
                                }
                            }
                        }
            }

            return result;
        }
        

        /// <summary>
        /// Создаёт и возвращает значение <see cref="ValueInfo"/>
        /// </summary>
        /// <param name="tagId">Id тега оборудования</param>
        /// <param name="date">Дата</param>
        /// <param name="timeStamp">Таймстамп значения</param>
        /// <param name="value">Значение</param>
        /// <returns></returns>
        private ValueInfo ValueInfoCreate(long tagId, DateTime date, DateTime timeStamp, double value) =>
            new()
            {
                IdTag = tagId,
                TimeStamp = timeStamp.TimeOfDay == TimeSpan.Zero
                    ? date.AddDays(1).Add(timeStamp.TimeOfDay)
                    : date.Add(timeStamp.TimeOfDay),
                Value = new ValueUnion
                {
                    ValueFloat = value,
                    IdQuality = "Normal"
                },
            };

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

        /// <summary>
        /// Возвращает код тега оборудования из параметров задания <see cref="parameters"/> по коду канала <see cref="channelCode"/>
        /// </summary>
        /// <param name="parameters">Параметры задания</param>
        /// <param name="channelCode">Код канала</param>
        /// <returns></returns>
        private string GetTagCode(Xml80020PointParameters parameters, string channelCode)
        {
            var channels = parameters.Channels.Where(t => t.ChannelCode == channelCode).ToList();
            return channels.Count switch
            {
                0 => throw new TaskException(TextParse.SelectedNoOneChannelsError.With(parameters.GetType().Name, channelCode)),
                1 => channels.Single().TagCode,
                _ => throw new TaskException(TextParse.SelectedManyChannelsError.With(parameters.GetType().Name, channels.Count, channelCode))
            };
        }

        /// <summary>
        /// Возвращает тег оборудования <see cref="LogicDevice"/> по коду <see cref="tagCode"/>
        /// </summary>
        /// <param name="logicDevice">Оборудование</param>
        /// <param name="tagCode">Код тега</param>
        /// <returns></returns>
        private Tag GetTag(LogicDevice logicDevice, string tagCode)
        {
            var tags = logicDevice.Tags.Where(t => t.LogicTagLink.LogicTagType.Code == tagCode).ToList();
            return tags.Count switch
            {
                0 => throw new TaskException(TextParse.SelectedNoOneTagsError.With(logicDevice.Id, tagCode)),
                1 => tags.Single(),
                _ => throw new TaskException(TextParse.SelectedManyTagsError.With(logicDevice.Id, tags.Count, tagCode))
            };
        }

        /// <summary>
        /// Возвращает тип таймстампа значений с таймстампами <see cref="dateTimeStart"/> и <see cref="dateTimeEnd"/>
        /// </summary>
        /// <param name="dateTimeStart">Таймштамп первого значения</param>
        /// <param name="dateTimeEnd">Таймштамп второго значения</param>
        /// <returns></returns>
        private TimeStampTypeEnum GetTimeStamp(DateTime dateTimeStart, DateTime dateTimeEnd)
        {
            var timeSpan = dateTimeEnd - dateTimeStart;
            return Math.Round(timeSpan.TotalMinutes) switch
            {
                30 => TimeStampTypeEnum.HalfHour,
                60 => TimeStampTypeEnum.Hour,
                _ => throw new TaskException(TextParse.UndefinedTimestampError.With(dateTimeStart, dateTimeEnd))
            };
        }

        /// <summary>
        /// Возвращает тип таймстампа значений с таймстампами <see cref="dateTimeStart"/> и <see cref="dateTimeEnd"/>
        /// </summary>
        /// <param name="dateTimeStart">Таймштамп первого значения</param>
        /// <param name="dateTimeEnd">Таймштамп второго значения</param>
        /// <returns></returns>
        private TimeStampTypeEnum GetTimeStamp(string dateTimeStart, string dateTimeEnd)
        {
            if (!DateTime.TryParse(dateTimeStart, out var start))
                throw new TaskException(TextParse.FailedToConvertToDateTimeError.With(dateTimeStart));
            if (!DateTime.TryParse(dateTimeEnd, out var end))
                throw new TaskException(TextParse.FailedToConvertToDateTimeError.With(dateTimeEnd));
            return GetTimeStamp(start, end);
        }
    }
}
