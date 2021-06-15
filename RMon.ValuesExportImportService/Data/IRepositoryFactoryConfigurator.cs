using RMon.Data.Provider.Esb.Backend;

namespace RMon.ValuesExportImportService.Data
{
    public interface IRepositoryFactoryConfigurator
    {
        /// <summary>
        /// Возвращает репозиторий для задач
        /// </summary>
        /// <returns></returns>
        IRepository TaskRepositoryCreate();
    }
}