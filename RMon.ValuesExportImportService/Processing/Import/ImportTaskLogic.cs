using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RMon.Configuration.Options;
using RMon.Core.Base;
using RMon.Core.MainServerInterface;
using RMon.Data.Provider;
using RMon.Data.Provider.Values;
using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesImportTaskDto;
using RMon.Globalization;
using RMon.Globalization.String;
using RMon.Values.ExportImport.Core;
using RMon.ValuesExportImportService.Extensions;
using RMon.ValuesExportImportService.Processing.Common;
using RMon.ValuesExportImportService.Processing.Extensions;
using RMon.ValuesExportImportService.ServiceBus;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Processing.Import
{
    class ImportTaskLogic : IImportTaskLogic
    {
        private readonly IOptionsMonitor<Service> _serviceOptions;

        private readonly IImportTaskLogger _taskLogger;
        private readonly ISimpleFactory<IValueRepository> _valueRepositorySimpleFactory;
        private readonly ITransformationRatioCalculator _transformationRatioCalculator;
        private readonly IResultMessagesSender _resultMessagesSender;
        private readonly IValuesLogger _valuesLogger;


        public ImportTaskLogic(IOptionsMonitor<Service> serviceOptions, 
            IImportTaskLogger taskLogger, 
            ISimpleFactory<IValueRepository> valueRepositorySimpleFactory, 
            ITransformationRatioCalculator transformationRatioCalculator,
            IResultMessagesSender resultMessagesSender,
            IValuesLogger valuesLogger)
        {
            _serviceOptions = serviceOptions;
            _taskLogger = taskLogger;
            _valueRepositorySimpleFactory = valueRepositorySimpleFactory;
            _transformationRatioCalculator = transformationRatioCalculator;
            _resultMessagesSender = resultMessagesSender;
            _valuesLogger = valuesLogger;
        }

        public async Task StartTaskAsync(ITask receivedTask, CancellationToken ct)
        {
            if (receivedTask is IValuesImportTask task)
            {
                var instanceName = _serviceOptions.CurrentValue.InstanceName;
                var dbTask = task.ToDbTask(instanceName);
                var context = new ImportProcessingContext(task, dbTask, _taskLogger, task.IdUser.Value);

                try
                {
                    await context.LogStarted(TextTask.Start).ConfigureAwait(false);
                    var values = task.Parameters.Values;
                    _valuesLogger.LogReceivedValues(task.CorrelationId, values);

                    var valueRepository = _valueRepositorySimpleFactory.Create();
                    await _transformationRatioCalculator.LoadTagsRatioFromDbAsync(values.Select(t => t.IdTag).ToList(), ct).ConfigureAwait(false);

                    var groupValues = values.GroupBy(t => t.IdTag);

                    var tasks = groupValues.Select(t => ProcessingTag(t, valueRepository, context, ct));
                    await Task.WhenAll(tasks).ConfigureAwait(false);

                    await context.LogFinished(TextTask.FinishSuccess).ConfigureAwait(false);
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

        private async Task ProcessingTag(IGrouping<long, ValueInfo> tagValue, IValueRepository valueRepository, ImportProcessingContext context, CancellationToken ct)
        {
            try
            {
                var tag = _transformationRatioCalculator.TagsRatio.SingleOrDefault(t => t.IdTag == tagValue.Key);
                if (tag != null)
                {
                    var message = CreateManualTagDataMessage(tag.IdTag);
                    foreach (var value in tagValue)
                    {
                        ct.ThrowIfCancellationRequested();
                        /*Удаление текущего значения*/
                        if (value.Rewrite)
                            await valueRepository.DeleteAsync(value.ToValue()).ConfigureAwait(false);

                        /*Применение коэффициента трансформации*/
                        value.Value.ValueFloat = value.Value.ValueFloat / tag.TransformationRatio / tag.Ratio - tag.Offset;

                        /*Формирование пакета для DPS*/
                        if (value.Value.ValueFloat.HasValue)
                            message.data.usd.Add(CreateTagValue(value.TimeStamp, tag.TagCode, value.Value.ValueFloat.Value));
                    }

                    await _resultMessagesSender.SendPacketAsync(message, ct).ConfigureAwait(false);
                    await context.LogInfo(TextImport.TagImportSuccess.With(tag.TagCode, tag.IdTag)).ConfigureAwait(false);
                }
                else
                    await context.LogError(TextImport.TagNotFoundError.With(tagValue.Key)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await context.LogError(e.ConcatExceptionMessage(TextImport.TagNotImportError.With(tagValue.Key))).ConfigureAwait(false);
            }
        }

        public void AbortTask(ITask receivedTask, StateMachineInstance instance) => instance.CancellationTokenSource.Cancel();


        /// <summary>
        /// Create uspd data for logic tags
        /// </summary>
        /// <param name="idTag">Logic tag Id</param>
        /// <returns></returns>
        private DAServerDataMessage CreateManualTagDataMessage(long idTag)
        {
            var message = new DAServerDataMessage();

            message.data.uspd.tS = DateTime.Now;
            message.data.uspd.isTymeSynk = true;
            message.data.uspd.props.Add("DevCode", "Manual");
            message.data.uspd.props.Add("IdTag", idTag.ToString());

            return message;
        }

        /// <summary>
        /// Create tag value
        /// </summary>
        /// <param name="timeStamp">Timestamp</param>
        /// <param name="tagCode">Tag name</param>
        /// <param name="value">Value</param>
        /// <returns></returns>
        private DAServerDeviceDataItem CreateTagValue(DateTime timeStamp, string tagCode, double value)
        {
            var usd = new DAServerDeviceDataItem
            {
                tS = timeStamp,
                isTymeSynk = true
            };
            usd.tags.Add(tagCode, value.ToString(CultureInfo.InvariantCulture));
            return usd;
        }
    }
}
