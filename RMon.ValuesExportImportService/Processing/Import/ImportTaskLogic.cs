using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RMon.Configuration.Options;
using RMon.Data.Provider;
using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesImportTaskDto;
using RMon.Globalization;
using RMon.Globalization.String;
using RMon.ValuesExportImportService.Processing.Common;
using RMon.ValuesExportImportService.Processing.Parse;
using RMon.ValuesExportImportService.ServiceBus;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Processing.Import
{
    class ImportTaskLogic : IImportTaskLogic
    {
        private readonly IOptionsMonitor<Service> _serviceOptions;

        private readonly IImportTaskLogger _taskLogger;


        public ImportTaskLogic(IOptionsMonitor<Service> serviceOptions, IImportTaskLogger taskLogger)
        {
            _serviceOptions = serviceOptions;
            _taskLogger = taskLogger;
        }

        public async Task StartTaskAsync(ITask receivedTask, CancellationToken cancellationToken)
        {
            if (receivedTask is IValuesImportTask task)
            {
                var instanceName = _serviceOptions.CurrentValue.InstanceName;
                var dbTask = task.ToDbTask(instanceName);
                var context = new ParseProcessingContext(task, dbTask, _taskLogger, task.IdUser.Value);

                try
                {
                    //task.Parameters.
                    //await context.LogStarted(TextParse.Start).ConfigureAwait(false);
                    //await context.LogInfo(TextParse.ValidateParameters).ConfigureAwait(false);
                    //ValidateParameters(task);

                    //await context.LogInfo(TextParse.LoadingFiles, 10).ConfigureAwait(false);
                    //var files = await ReceiveFilesAsync(task.Parameters.Files, ct).ConfigureAwait(false);

                    //var values = task.Parameters.FileFormatType switch
                    //{
                    //    ValuesParseFileFormatType.Xml80020 => await _parseXml80020Logic.AnalyzeAsync(files, task.Parameters.Xml80020Parameters, context, ct).ConfigureAwait(false),
                    //    ValuesParseFileFormatType.Matrix24X31 => await _parseMatrix24X31Logic.AnalyzeAsync(files, task.Parameters.Matrix24X31Parameters, context, ct).ConfigureAwait(false),
                    //    ValuesParseFileFormatType.Matrix31X24 => await _parseMatrix31X24Logic.AnalyzeAsync(files, task.Parameters.Matrix31X24Parameters, context, ct).ConfigureAwait(false),
                    //    ValuesParseFileFormatType.Table => await _parseTableLogic.AnalyzeAsync(files, task.Parameters.TableParameters, context, ct).ConfigureAwait(false),
                    //    ValuesParseFileFormatType.Flexible => await _parseFlexibleFormatLogic.AnalyzeAsync(files, context, ct).ConfigureAwait(false),
                    //    _ => throw new ArgumentOutOfRangeException(),
                    //};
                    //await context.LogInfo(TextParse.LoadingCurrentValues, 70).ConfigureAwait(false);
                    //await LoadCurrentValuesFromDb(context, values).ConfigureAwait(false);

                    //await context.LogFinished(TextParse.FinishSuccess, values).ConfigureAwait(false);
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
    }
}
