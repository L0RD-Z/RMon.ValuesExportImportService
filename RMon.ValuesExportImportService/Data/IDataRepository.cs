using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RMon.Context.EntityStore;
using RMon.Data.Provider.Units.Backend.Common;

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

        /// <summary>
        /// Выполняет поиск тегов для указанного оборудования с указанными значениями свойств
        /// </summary>
        /// <param name="idUserGroups">Группа пользователей</param>
        /// <param name="idLogicDevice">Id оборудования</param>
        /// <param name="entityFilter">Коды свойств со значениями</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Список id тегов</returns>
        Task<IList<long>> FindTags(IList<long> idUserGroups, long idLogicDevice, Entity entityFilter, CancellationToken ct = default);
    }
}
