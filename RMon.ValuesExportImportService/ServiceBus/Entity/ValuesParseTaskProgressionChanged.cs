using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesParseTaskDto;

namespace RMon.ValuesExportImportService.ServiceBus.Entity
{
    /// <inheritdoc cref="IValuesParseTaskProgressChanged"/>
    class ValuesParseTaskProgressionChanged : BaseTask, IValuesParseTaskProgressChanged
    {
        public float Progress { get; set; }


        public ValuesParseTaskProgressionChanged(ITask task, float progress)
            :base(task)
        {
            Progress = progress;
        }
    }
}
