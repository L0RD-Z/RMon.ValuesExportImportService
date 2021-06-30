using System;
using System.Threading.Tasks;
using RMon.Data.Provider.Esb.Entities;
using RMon.ESB.Core.Common;
using RMon.Globalization.String;

namespace RMon.ValuesExportImportService.Processing.Common
{
    public interface IBaseTaskLogger<in T> where T : DbTask
    {
        /// <summary>
        /// Выполняет логирование информационного сообщения <see cref="msg"/> в лог-файл, в БД и в ESB
        /// </summary>
        /// <param name="dbTask"></param>
        /// <param name="msg">Текстовое сообщение</param>
        /// <param name="receivedTask"></param>
        /// <param name="level"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        Task Log(ITask receivedTask, T dbTask, I18nString msg, LogLevel level, float? progress);

        /// <summary>
        /// Выполняет логирование сообщения <see cref="msg"/> о старте выполнения задачи в лог-файл, в БД и в ESB 
        /// </summary>
        /// <param name="dbTask"></param>
        /// <param name="msg">Текстовое сообщение</param>
        /// <param name="receivedTask"></param>
        /// <returns></returns>
        Task LogStartedAsync(ITask receivedTask, T dbTask, I18nString msg);

        /// <summary>
        /// Выполняет логирование сообщения <see cref="msg"/> об отмене выполнения задачи в лог-файл, в БД и в RabbitMQ
        /// </summary>
        /// <param name="dbTask"></param>
        /// <param name="msg">Текстовое сообщение</param>
        /// <param name="receivedTask"></param>
        /// <returns></returns>
        Task LogAbortedAsync(ITask receivedTask, T dbTask, I18nString msg);

        /// <summary>
        /// Выполняет логирование сообщения <see cref="msg"/> об ошибке выполнения задачи в лог-файл, в БД и в RabbitMQ
        /// </summary>
        /// <param name="dbTask"></param>
        /// <param name="msg">Текстовое сообщение</param>
        /// <param name="ex"></param>
        /// <param name="receivedTask"></param>
        /// <returns></returns>
        Task LogFailedAsync(ITask receivedTask, T dbTask, I18nString msg, Exception ex);
    }
}