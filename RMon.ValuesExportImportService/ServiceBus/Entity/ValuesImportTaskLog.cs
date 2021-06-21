using System;
using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesImportTaskDto;

namespace RMon.ValuesExportImportService.ServiceBus.Entity
{
    /// <inheritdoc cref="IValuesImportTaskLog"/>
    class ValuesImportTaskLog : BaseTask, IValuesImportTaskLog
    {
        public DateTime DateTime { get; set; }
        public LogLevel Level { get; set; }
        public string Text { get; set; }
        public string Data { get; set; }


        public ValuesImportTaskLog(ITask task, DateTime dateTime, LogLevel level, string text, string data)
            :base(task)
        {
            DateTime = dateTime;
            Level = level;
            Text = text;
            Data = data;
        }
    }
}
