using System;
using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesExportTaskDto;

namespace RMon.ValuesExportImportService.ServiceBus.Entity
{
    /// <inheritdoc cref="IValuesExportTaskLog"/>
    class ValuesExportTaskLog : BaseTask, IValuesExportTaskLog
    {
        public DateTime DateTime { get; set; }
        public LogLevel Level { get; set; }
        public string Text { get; set; }
        public string Data { get; set; }


        public ValuesExportTaskLog(ITask task, DateTime dateTime, LogLevel level, string text, string data)
            :base(task)
        {
            DateTime = dateTime;
            Level = level;
            Text = text;
            Data = data;
        }
    }
}
