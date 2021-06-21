using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RMon.Core.CommonTask;
using RMon.ESB.Core.Common;
using RMon.Values.ExportImport.Core;
using RMon.ValuesExportImportService.ServiceBus.Common;

namespace RMon.ValuesExportImportService.ServiceBus.Parse
{
    interface IParseBusPublisher:IBusPublisher
    {
        /// <summary>
        /// Отправляет уведомление на шину RabbitMQ о том, что выполнение задачи <see cref="receivedTask"/> завершилось
        /// </summary>
        /// <param name="receivedTask">Полученная задача</param>
        /// <param name="date">Дата и время</param>
        /// <param name="instanceName"></param>
        /// <param name="state">Статус завершения задачи</param>
        /// <param name="values">Список значений для записи в БД</param>
        /// <returns></returns>
        Task SendTaskFinishedAsync(ITask receivedTask, DateTime date, string instanceName, TaskState state, IList<ValueInfo> values);
    }
}
