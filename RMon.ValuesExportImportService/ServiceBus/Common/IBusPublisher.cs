using System;
using System.Threading.Tasks;
using RMon.Core.CommonTask;
using RMon.ESB.Core.Common;

namespace RMon.ValuesExportImportService.ServiceBus.Common
{
    public interface IBusPublisher
    {
        /// <summary>
        /// Отправляет уведомление на шину RabbitMQ о том, что начато выполнение задачи <see cref="receivedTask"/>
        /// </summary>
        /// <param name="receivedTask">Полученная задача</param>
        /// <param name="date">Дата и время</param>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        Task SendTaskStartedAsync(ITask receivedTask, DateTime date, string instanceName);

        /// <summary>
        /// Отправляет уведомление на шину RabbitMQ о том, что при выполнении задачи <see cref="receivedTask"/> возникла ситуация <see cref="message"/>
        /// </summary>
        /// <param name="receivedTask">Полученная задача</param>
        /// <param name="date">Дата и время</param>
        /// <param name="message">Сообщение информационного характера</param>
        /// <returns></returns>
        Task SendTaskLogAsync(ITask receivedTask, DateTime date, string message);

        /// <summary>
        /// Отправляет уведомление на шину RabbitMQ о том, что при выполнении задачи <see cref="receivedTask"/> возникла ситуация <see cref="message"/>
        /// </summary>
        /// <param name="receivedTask">Полученная задача</param>
        /// <param name="date">Дата и время</param>
        /// <param name="logLevel"></param>
        /// <param name="message">Сообщение информационного характера</param>
        /// <returns></returns>
        Task SendTaskLogAsync(ITask receivedTask, DateTime date, LogLevel logLevel, string message);

        /// <summary>
        /// Отправляет уведомление на шину RabbitMQ о том, что при выполнении задачи <see cref="receivedTask"/> возникло исключение
        /// </summary>
        /// <param name="receivedTask">Полученная задача</param>
        /// <param name="date">Дата и время</param>
        /// <param name="message">Сообщение об исключении</param>
        /// <param name="exception">Вызванное исключение</param>
        /// <returns></returns>
        Task SendTaskLogAsync(ITask receivedTask, DateTime date, string message, Exception exception);

        /// <summary>
        /// Отправляет уведомление на шину RabbitMQ о том, что задача <see cref="receivedTask"/> выполнена на <see cref="progress"/> пунктов
        /// </summary>
        /// <param name="receivedTask">Полученная задача</param>
        /// <param name="progress">Процент выполнения задачи <see cref="receivedTask"/> в диапазоне [0; 100]</param>
        /// <returns></returns>
        Task SendTaskProgressionChangedAsync(ITask receivedTask, float progress);

        /// <summary>
        /// Отправляет уведомление на шину RabbitMQ о том, что выполнение задачи <see cref="receivedTask"/> завершилось
        /// </summary>
        /// <param name="receivedTask">Полученная задача</param>
        /// <param name="date">Дата и время</param>
        /// <param name="instanceName"></param>
        /// <param name="state">Статус завершения задачи</param>
        /// <returns></returns>
        Task SendTaskFinishedAsync(ITask receivedTask, DateTime date, string instanceName, TaskState state);
    }
}
