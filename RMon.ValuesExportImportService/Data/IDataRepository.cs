using System;
using System.Collections.Generic;
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
        /// Возвращает список тегов
        /// </summary>
        /// <param name="idLogicDevices">Список id устройств</param>
        /// <param name="tagCodes">Список кодов тегов</param>
        /// <returns></returns>
        Task<IList<Tag>> GetTagsAsync(IList<long> idLogicDevices, IList<string> tagCodes);
    }
}
