using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RMon.Context.EntityStore;
using RMon.DriverCore;
using RMon.Values.ExportImport.Core;
using RMon.Values.ExportImport.Core.FileFormatParameters;
using RMon.ValuesExportImportService.Data;
using RMon.ValuesExportImportService.Extensions;
using RMon.ValuesExportImportService.Files;
using RMon.ValuesExportImportService.Processing.Common;
using RMon.ValuesExportImportService.Processing.Parse.Format80020;
using RMon.ValuesExportImportService.Processing.Parse.Format80020.Models;
using RMon.ValuesExportImportService.Text;
using DateTime = System.DateTime;
using Message = RMon.ValuesExportImportService.Processing.Parse.Format80020.Models.Message;

namespace RMon.ValuesExportImportService.Processing.Parse
{
    class ParseXml80020Logic
    {
        private readonly IDataRepository _dataRepository;

        private readonly List<TimeStampTypeEnum> _supportedTimestampTypes = new()
        {
            TimeStampTypeEnum.HalfHour, TimeStampTypeEnum.Hour
        };

        public ParseXml80020Logic(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        /// <summary>
        /// Асинхронно выполняет анализ файлов в формате 80020
        /// </summary>
        /// <param name="files">Список файлов</param>
        /// <param name="taskParams">Параметры задания</param>
        /// <param name="context">Контекст выполнения</param>
        /// <param name="ct">Токен отмены опреации</param>
        /// <returns></returns>
        public async Task<List<ValueInfo>> AnalyzeAsync(IList<LocalFile> files, Xml80020ParsingParameters taskParams, ParseProcessingContext context, CancellationToken ct)
        {
            var messages = new List<(string FileName, Message)>();
            foreach (var file in files)
            {
                await context.LogInfo(TextParse.ReadingFile.With(file.Path, ValuesParseFileFormatType.Xml80020.ToString())).ConfigureAwait(false);

                var message = Parser.Parse(file.Body);
                messages.Add((file.Path, message));
            }

            var result = new List<ValueInfo>();
            foreach (var message in messages)
            {
                await context.LogInfo(TextParse.AnalyzeInfoFromFile.With(message.FileName)).ConfigureAwait(false);
                var date = DateTime.ParseExact(message.Item2.DateTime.Day, "yyyyMMdd", CultureInfo.InvariantCulture);

                foreach (var area in message.Item2.Areas)
                {
                    await context.LogInfo(TextParse.AnalyzeArea.With(area.Name, area.Inn)).ConfigureAwait(false);
                    if (taskParams.MeasuringPoint != null)
                    {
                        await context.LogInfo(TextParse.AnalyzeMeasuringPoints).ConfigureAwait(false);
                        var values = await AnalyzePointsAsync(area.Name, area.MeasuringPoints, taskParams.MeasuringPoint, date, context, message.FileName, ct).ConfigureAwait(false);
                        result.AddRange(values);
                    }

                    if (taskParams.DeliveryPoint != null)
                    {
                        await context.LogInfo(TextParse.AnalyzeDeliveryPoints).ConfigureAwait(false);
                        var values = await AnalyzePointsAsync(area.Name, area.DeliveryPoints, taskParams.DeliveryPoint, date, context, message.FileName, ct).ConfigureAwait(false);
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
        private async Task<List<ValueInfo>> AnalyzePointsAsync(string areaName, MeasuringPoint[] points, Xml80020PointParameters pointParameters, DateTime date, ParseProcessingContext context, string fileName, CancellationToken ct)
        {
            var result = new List<ValueInfo>();
            if (points != null && points.Any())
                foreach (var measuringPoint in points)
                {
                    await context.LogInfo(TextParse.AnalyzePoint.With(measuringPoint.Name, measuringPoint.Code)).ConfigureAwait(false);
                    var logicDevice = await _dataRepository.GetLogicDeviceByPropertyValueAsync(pointParameters.PointPropertyCode, measuringPoint.Code, ct).ConfigureAwait(false);
                    foreach (var channel in pointParameters.Channels)
                    {
                        var measuringChannel = GetChannel(measuringPoint, channel.ChannelCode);
                        result.AddRange(AnalyzeChannel(measuringChannel, channel.TagCode, logicDevice, date, context));
                    }
                }
            else
                await context.LogWarning(TextParse.NotFoundSectionWarning.With(fileName, areaName, nameof(DeliveryPoint))).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Выполняет парсинг канала поставки
        /// </summary>
        /// <param name="measuringChannel">Канал</param>
        /// <param name="tagCode">Код тега</param>
        /// <param name="logicDevice">Логическое устройство</param>
        /// <param name="date">Дата</param>
        /// <param name="processingContext">Контекст выполнения задания</param>
        /// <returns></returns>
        private List<ValueInfo> AnalyzeChannel(MeasuringChannel measuringChannel, string tagCode, LogicDevice logicDevice, DateTime date, ParseProcessingContext processingContext)
        {
            var result = new List<ValueInfo>();
            var tag = logicDevice.Tags.SingleOrDefault(t => t.LogicTagLink.LogicTagType.Code == tagCode);
            if (tag == null)
                throw new TaskException(TextParse.SelectedNoOneTagsError.With(logicDevice.Id, tagCode));

            var timeStampTypeTag = TimeStampTypeEnum.GetTypeByValue(tag.DeviceTag.IdTimeStampType);
            if (!_supportedTimestampTypes.Contains(timeStampTypeTag))
            {
                var supportedTimestamps = string.Join(", ", _supportedTimestampTypes.Select(t => $"\"{t}\""));
                throw new TaskException(TextParse.MissingTimestampTypeError.With(tagCode, supportedTimestamps));
            }

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
                                    if ((currentValue.TimeStamp - lastValue.TimeStamp).TotalMinutes == 30)
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
        /// Возвращает канал с кодом <see cref="channelCode"/>
        /// </summary>
        /// <param name="measuringPoint">Точка поставки/измерения</param>
        /// <param name="channelCode">Код канала</param>
        /// <returns></returns>
        private MeasuringChannel GetChannel(MeasuringPoint measuringPoint, string channelCode)
        {
            var channels = measuringPoint.MeasuringChannels.Where(t => t.Code == channelCode).ToList();
            return channels.Count switch
            {
                0 => throw new TaskException(TextParse.SelectedNoOneChannelsError.With(measuringPoint.Name, measuringPoint.Code, channelCode)),
                1 => channels.Single(),
                _ => throw new TaskException(TextParse.SelectedManyChannelsError.With(measuringPoint.Name, measuringPoint.Code, channelCode))
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
            dateTimeEnd = dateTimeEnd.TimeOfDay == TimeSpan.Zero ? dateTimeEnd.AddDays(1) : dateTimeEnd;
            var timeSpan = dateTimeEnd - dateTimeStart;
            return Math.Round(timeSpan.TotalMinutes) switch
            {
                30 => TimeStampTypeEnum.HalfHour,
                60 => TimeStampTypeEnum.Hour,
                _ => throw new TaskException(TextParse.UndefinedTimestampError.With(dateTimeStart, dateTimeEnd))
            };
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
            new(tagId, timeStamp.TimeOfDay == TimeSpan.Zero ? date.AddDays(1).Add(timeStamp.TimeOfDay) : date.Add(timeStamp.TimeOfDay), value);
    }
}
