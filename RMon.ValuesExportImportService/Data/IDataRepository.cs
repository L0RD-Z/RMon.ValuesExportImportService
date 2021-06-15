using System;
using System.Threading.Tasks;

namespace RMon.ValuesExportImportService.Data
{
    public interface IDataRepository
    {
        /// <summary>
        /// Возвращает текущую дату и время
        /// </summary>
        /// <returns></returns>
        Task<DateTime> GetDateAsync();
    }
}
