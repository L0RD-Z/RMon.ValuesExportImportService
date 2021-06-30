using System.Collections.Generic;
using System.Threading.Tasks;
using RMon.Data.Provider.Esb.Entities.ValuesExportImport;
using RMon.ESB.Core.Common;
using RMon.Globalization.String;
using RMon.Values.ExportImport.Core;
using RMon.ValuesExportImportService.Processing.Common;

namespace RMon.ValuesExportImportService.Processing.Parse
{
    internal interface IParseTaskLogger : IBaseTaskLogger<DbValuesExportImportTask>
    {
        /// <summary>
        /// Выполняет логирование сообщения <see cref="msg"/> о успешном завершении выполнения задачи в лог-файл, в БД и в RabbitMQ
        /// </summary>
        /// <param name="receivedTask"></param>
        /// <param name="dbTask"></param>
        /// <param name="msg">Текстовое сообщение</param>
        /// <param name="values">Список считанных значений</param>
        /// <returns></returns>
        Task LogFinishedAsync(ITask receivedTask, DbValuesExportImportTask dbTask, I18nString msg, IList<ValueInfo> values);
    }
}