using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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
using RMon.ValuesExportImportService.Processing.Parse.Format80020.Entity;
using RMon.ValuesExportImportService.Text;
using DateTime = System.DateTime;
using Message = RMon.ValuesExportImportService.Processing.Parse.Format80020.Entity.Message;

namespace RMon.ValuesExportImportService.Processing.Parse
{
    class Parse80020Logic
    {
        private readonly IDataRepository _dataRepository;

        private readonly List<TimeStampTypeEnum> _supportedTimestampTypes = new()
        {
            TimeStampTypeEnum.HalfHour, TimeStampTypeEnum.Hour
        };

        public Parse80020Logic(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        /// <summary>
        /// Асинхронно выполняет парсинг файлов в формате 80020
        /// </summary>
        /// <param name="files">Список файлов</param>
        /// <param name="taskParams">Параметры задания</param>
        /// <param name="context">Контекст выполнения</param>
        /// <param name="ct">Токен отмены опреации</param>
        /// <returns></returns>
        public async Task<List<ValueInfo>> ParseFormat80020Async(IList<LocalFile> files, Xml80020ParsingParameters taskParams, ParseProcessingContext context, CancellationToken ct)
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
                    var logicDevice = await _dataRepository.GetLogicDeviceByPropertyValueAsync(pointParameters.PointPropertyCode, measuringPoint.Code, ct).ConfigureAwait(false);
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
