using System;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RMon.ESB.Core.Common;

namespace RMon.ValuesExportImportService.ServiceBus.Common
{
    static class BusLoggerExtension
    {
        /// <summary>
        /// Логирует полученные задачи
        /// </summary>
        /// <param name="logger">Логгер <see cref="ILogger"/> для записи</param>
        /// <param name="task">Полученная задача</param>
        /// <param name="taskType">Тип задачи</param>
        public static void LogReceivedTask(this ILogger<BusLogger> logger, ITask task, Type taskType) => logger.LogTask(task, taskType, TaskActions.Received);

        /// <summary>
        /// Логирует отправленные задачи
        /// </summary>
        /// <param name="logger">Логгер <see cref="ILogger"/> для записи</param>
        /// <param name="task">Полученная задача</param>
        /// <param name="taskType">Тип задачи</param>
        public static void LogSentTask(this ILogger<BusLogger> logger, ITask task, Type taskType) => logger.LogTask(task, taskType, TaskActions.Sent);


        static void LogTask (this ILogger<BusLogger> logger, ITask task, Type taskType, TaskActions action)
        {
            var serializeObject = JsonConvert.SerializeObject(task, Formatting.Indented);
            var logMessages = new StringBuilder();
            logMessages.AppendLine($"{taskType.FullName} {(action == TaskActions.Sent ? "отправлено": "получено")}");
            logMessages.AppendLine(serializeObject);
            logger.Log(Microsoft.Extensions.Logging.LogLevel.Information, logMessages.ToString());
        }

        private enum TaskActions
        {
            Sent,
            Received
        }
    }
}
