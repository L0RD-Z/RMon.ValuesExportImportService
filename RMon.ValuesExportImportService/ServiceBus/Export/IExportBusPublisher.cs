using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RMon.Core.CommonTask;
using RMon.Core.Files;
using RMon.ESB.Core.Common;
using RMon.ValuesExportImportService.ServiceBus.Common;

namespace RMon.ValuesExportImportService.ServiceBus.Export
{
    interface IExportBusPublisher : IBusPublisher
    {
        /// <summary>
        /// Отправляет уведомление на шину RabbitMQ о том, что выполнение задачи <see cref="receivedTask"/> завершилось
        /// </summary>
        /// <param name="receivedTask">Полученная задача</param>
        /// <param name="date">Дата и время</param>
        /// <param name="instanceName"></param>
        /// <param name="state">Статус завершения задачи</param>
        /// <param name="files">Список ссылок на сгенерированные файлы</param>
        /// <returns></returns>
        Task SendTaskFinishedAsync(ITask receivedTask, DateTime date, string instanceName, TaskState state, IList<FileInStorage> files);
    }
}
