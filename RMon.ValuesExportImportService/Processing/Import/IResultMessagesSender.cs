﻿using System.Threading;
using System.Threading.Tasks;
using RMon.Core.MainServerInterface;

namespace RMon.ValuesExportImportService.Processing.Import
{
    /// <summary>
    /// интерфейс для отправки (сохранения) сообщений с результатами
    /// </summary>
    public interface IResultMessagesSender
    {
        /// <summary>
        /// Отправка (сохранение) пакета с результатами
        /// </summary>
        /// <param name="packet">Пакет</param>
        /// <param name="idSsdList"></param>
        /// <param name="idAnalysisService"></param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Id импортированного пакета</returns>
        Task<long> SendPacketAsync(DAServerDataMessage packet, long idSsdList = 0, string idAnalysisService = "", CancellationToken ct = default);
        bool IsServiceAvailable();
    }
}
