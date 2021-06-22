using System;
using System.Collections.Generic;
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
                            var messages = new List<(string fileName, Message)>();
                            foreach (var file in files)
                            {
                                await context.LogInfo(TextParse.ReadingFile.With(file.Path, task.Parameters.FileFormatType.ToString())).ConfigureAwait(false);

                                var message = Parser.Parse(file.Body);
                                messages.Add((file.Path, message));
                            }

                            
                            var taskParams = task.Parameters.Xml80020Parameters;
                            foreach (var message in messages)
                                foreach (var area in message.Item2.Areas)
                                {
                                    if (taskParams.MeasuringPoint != null)
                                        if (area.MeasuringPoints.Any())
                                            foreach (var measuringPoint in area.MeasuringPoints)
                                            {
                                                //Todo как лучше: тянуть один logicDevice со всеми сущностями или тянуть перед этим все IdlogicDevices и по каждому из них вытаскивать tag со всеми сущностями
                                                var logicDevice = await DataRepository.GetLogicDeviceByPropertyValueAsync(taskParams.MeasuringPoint.PointPropertyCode, measuringPoint.Code, ct).ConfigureAwait(false);

                                                foreach (var measuringChannel in measuringPoint.MeasuringChannels)
                                                {
                                                    var tagCode = GetTagCode(taskParams.MeasuringPoint, measuringChannel.Code);
                                                    var tag = GetTag(logicDevice, tagCode);

                                                    var timeStampTypeTag = TimeStampTypeEnum.GetTypeByValue(tag.DeviceTag.IdTimeStampType);
                                                    
                                                    foreach (var period in measuringChannel.Periods)
                                                    {
                                                        if (!DateTime.TryParse(period.Start, out var dateTimeStart))
                                                            throw new TaskException(TextParse.FailedToConvertToDateTimeError.With(period.Start));
                                                        if (!DateTime.TryParse(period.End, out var dateTimeEnd))
                                                            throw new TaskException(TextParse.FailedToConvertToDateTimeError.With(period.End));
                                                        var timeStampTypeValue = GetTimeStamp(dateTimeStart, dateTimeEnd);

                                                        if (timeStampTypeTag == timeStampTypeValue)
                                                        {
                                                            var valueInfo = new ValueInfo()
                                                            {
                                                                IdTag = tag.Id,
                                                                TimeStamp = dateTimeStart, //Todo или dateTimeEnd?
                                                                Value = new ValueUnion() //Todo как быть с остальными типами значений
                                                                {
                                                                    ValueData = period.Value.Val
                                                                }
                                                            };
                                                        }
                                                    }
                                                }
                                            }
                                        else
                                            await context.LogWarning(TextParse.NotFoundSectionWarning.With(message.fileName, area.Name, nameof(MeasuringPoint))).ConfigureAwait(false);
                                    
                                    if (taskParams.DeliveryPoint != null)
                                        if (area.DeliveryPoints.Any())
                                        {
                                            foreach (var deliveryPoint in area.DeliveryPoints)
                                            {
                                                var logicDevice = await DataRepository.GetLogicDeviceByPropertyValueAsync(taskParams.DeliveryPoint.PointPropertyCode, deliveryPoint.Code, ct).ConfigureAwait(false);
                                            }
                                        }
                                        else
                                            await context.LogWarning(TextParse.NotFoundSectionWarning.With(message.fileName, area.Name, nameof(DeliveryPoint))).ConfigureAwait(false);
                                }
                            
                            


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
