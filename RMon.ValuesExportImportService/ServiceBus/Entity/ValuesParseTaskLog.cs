using System;
using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesParseTaskDto;

namespace RMon.ValuesExportImportService.ServiceBus.Entity
{
    /// <inheritdoc cref="IValuesParseTaskLog"/>
    class ValuesParseTaskLog : BaseTask, IValuesParseTaskLog
    {
        public DateTime DateTime { get; set; }
        public LogLevel Level { get; set; }
        public string Text { get; set; }
        public string Data { get; set; }


        public ValuesParseTaskLog(ITask task, DateTime dateTime, LogLevel level, string text, string data)
            :base(task)
        {
            DateTime = dateTime;
            Level = level;
            Text = text;
            Data = data;
        }
    }
}
