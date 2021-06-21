using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesImportTaskDto;

namespace RMon.ValuesExportImportService.ServiceBus.Entity
{
    /// <inheritdoc cref="IValuesImportTaskProgressChanged"/>
    class ValuesImportTaskProgressionChanged : BaseTask, IValuesImportTaskProgressChanged
    {
        public float Progress { get; set; }


        public ValuesImportTaskProgressionChanged(ITask task, float progress)
            :base(task)
        {
            Progress = progress;
        }
    }
}
