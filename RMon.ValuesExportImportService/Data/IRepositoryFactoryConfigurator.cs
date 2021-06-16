using RMon.Data.Provider.Esb.Backend;
using RMon.Security.Core;

namespace RMon.ValuesExportImportService.Data
{
    public interface IRepositoryFactoryConfigurator
    {
        /// <summary>
        /// Возвращает репозиторий  для работы с Правами доступа
        /// </summary>
        /// <returns></returns>
        IPermissionProvider PermissionProviderCreate();

        /// <summary>
        /// Возвращает репозиторий  для работы с Пользователями
        /// </summary>
        /// <returns></returns>
        RMon.Data.Provider.Security.IRepository UsersRepositoryCreate(IPermissionProvider permissionProvider);

        /// <summary>
        /// Возвращает репозиторий для задач
        /// </summary>
        /// <returns></returns>
        IRepository TaskRepositoryCreate();
    }
}