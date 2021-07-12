using System;
using System.Collections.Generic;
using RMon.Values.ExportImport.Core;

namespace RMon.ValuesExportImportService.Processing.Parse
{
    internal interface IValuesLogger
    {
        void LogValues(Guid correlationId, IList<ValueInfo> values);
    }
}