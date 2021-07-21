using System;
using System.Collections.Generic;
using RMon.Values.ExportImport.Core;

namespace RMon.ValuesExportImportService.Processing.Common
{
    internal interface IValuesLogger
    {
        /// <summary>
        /// Выполняет логирование значений <see cref="values"/> после получения
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="values"></param>
        void LogReceivedValues(Guid correlationId, IList<ValueInfo> values);
        
        /// <summary>
        /// Выполняет логирование значений <see cref="values"/> перед отправкой
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="values"></param>
        void LogSendValues(Guid correlationId, IList<ValueInfo> values);
    }
}