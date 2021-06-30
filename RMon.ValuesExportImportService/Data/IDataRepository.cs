using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RMon.Context.EntityStore;

namespace RMon.ValuesExportImportService.Data
{
    public interface IDataRepository
    {
        /// <summary>
        /// Возвращает текущую дату и время
        /// </summary>
        /// <returns></returns>
        Task<DateTime> GetDateAsync();

        /// <summary>
        /// Асинхронно возвращает список тегов с кодами <see cref="tagCodes"/> для оборудования <see cref="idLogicDevices"/>
        /// </summary>
        /// <param name="idLogicDevices">Список id устройств</param>
        /// <param name="tagCodes">Список кодов тегов</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns></returns>
        Task<IList<Tag>> GetTagsAsync(IList<long> idLogicDevices, IList<string> tagCodes, CancellationToken cancellationToken);

        /// <summary>
        /// Асинхронно возвращает логическое устройство из БД, значение свойства <see cref="propertyCode"/> которого равно <see cref="propertyCode"/>
        /// </summary>
        /// <param name="propertyCode">Код свойства оборудования</param>
        /// <param name="propertyValue">Значение свойства оборудования</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns></returns>
        Task<LogicDevice> GetLogicDeviceByPropertyValueAsync(string propertyCode, string propertyValue, CancellationToken cancellationToken);
    }
}
