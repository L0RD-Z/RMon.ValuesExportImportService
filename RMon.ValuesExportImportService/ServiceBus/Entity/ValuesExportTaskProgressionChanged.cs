using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesExportTaskDto;

namespace RMon.ValuesExportImportService.ServiceBus.Entity
{
    /// <inheritdoc cref="IValuesExportTaskProgressChanged"/>
    class ValuesExportTaskProgressionChanged : BaseTask, IValuesExportTaskProgressChanged
    {
        public float Progress { get; set; }


        public ValuesExportTaskProgressionChanged(ITask task, float progress)
            :base(task)
        {
            Progress = progress;
        }
    }
}
