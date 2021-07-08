using System.Threading;
using System.Threading.Tasks;
using RMon.ESB.Core.Common;
using RMon.ValuesExportImportService.ServiceBus;

namespace RMon.ValuesExportImportService.Processing
{
    interface ITaskLogic
    {
        /// <summary>
        /// Выполнение задачи <see cref="receivedTask"/>
        /// </summary>
        /// <param name="receivedTask">Полученная задача</param>
        /// <param name="ct">Токен отмены задачи</param>
        /// <returns></returns>
        Task StartTaskAsync(ITask receivedTask, CancellationToken ct);

        /// <summary>
        /// Выполняет отмену задачи <see cref="receivedTask"/>
        /// </summary>
        /// <param name="receivedTask">Полученная задача</param>
        /// <param name="instance"></param>
        void AbortTask(ITask receivedTask, StateMachineInstance instance);
    }
}
