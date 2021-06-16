using System.Threading.Tasks;
using RMon.Commissioning.Core;

namespace RMon.ValuesExportImportService.Processing.Permission
{
    /// <summary>
    /// Логика работы с прадвами доступа
    /// </summary>
    public interface IPermissionLogic
    {
        /// <summary>
        /// Возвращает список Id групп пользователя <see cref="idUser"/> с правами доступа
        /// </summary>
        /// <param name="entityType">Тип сущности</param>
        /// <param name="crudOperation">Тип операции</param>
        /// <param name="idUser">Id пользователя</param>
        /// <returns></returns>
        Task<long[]> GetUserGroupIdsWithPermissionAsync(EntityTypes entityType, CrudOperations crudOperation, long idUser);
    }
}