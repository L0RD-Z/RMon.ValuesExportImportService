using System.Threading.Tasks;
using RMon.Data.Provider.Esb.Entities.ValuesExportImport;
using RMon.ESB.Core.Common;
using RMon.Globalization.String;
using RMon.ValuesExportImportService.Processing.Common;

namespace RMon.ValuesExportImportService.Processing.Import
{
    internal interface IImportTaskLogger : IBaseTaskLogger<DbValuesExportImportTask>
    {
        /// <summary>
        /// Выполняет логирование сообщения <see cref="msg"/> о успешном завершении выполнения задачи в лог-файл, в БД и в RabbitMQ
        /// </summary>
        /// <param name="receivedTask"></param>
        /// <param name="dbTask"></param>
        /// <param name="msg">Текстовое сообщение</param>
        /// <returns></returns>
        Task LogFinishedAsync(ITask receivedTask, DbValuesExportImportTask dbTask, I18nString msg);
    }
}