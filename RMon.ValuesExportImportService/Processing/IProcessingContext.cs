using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RMon.Core.Files;
using RMon.ESB.Core.Common;
using RMon.Globalization;
using RMon.Globalization.String;

namespace RMon.ValuesExportImportService.Processing
{
    interface IProcessingContext
    {
        long IdUser { get; set; }

        IGlobalizationProvider GlobalizationProvider { get; set; }

        Task LogStarted(I18nString msg);

        Task LogAborted(I18nString msg);

        Task LogFailed(I18nString msg, Exception ex);

        Task Log(I18nString msg, LogLevel logLevel, float? progress);
    }
}
