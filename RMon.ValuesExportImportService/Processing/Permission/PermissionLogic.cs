using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RMon.ValuesExportImportService.Data;

namespace RMon.ValuesExportImportService.Processing.Permission
{
    public class PermissionLogic : IPermissionLogic
    {
        private readonly IRepositoryFactoryConfigurator _repositoryFactoryConfigurator;

        private readonly List<PermissionConstant> _permissionConstants = new()
        {
            new PermissionConstant(EntityTypes.Values, CrudOperations.Read, "DP_ALLOW"),
            new PermissionConstant(EntityTypes.Values, CrudOperations.Create, "DP_EDIT_VALUE"),
        };

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="repositoryFactoryConfigurator">Конфигуратор репозиториев</param>
        public PermissionLogic(IRepositoryFactoryConfigurator repositoryFactoryConfigurator)
        {
            _repositoryFactoryConfigurator = repositoryFactoryConfigurator;
        }


        public async Task<long[]> GetUserGroupIdsWithPermissionAsync(EntityTypes entityType, CrudOperations crudOperation, long idUser)
        {
            var permissionProvider = _repositoryFactoryConfigurator.PermissionProviderCreate();
            var usersRepository = _repositoryFactoryConfigurator.UsersRepositoryCreate(permissionProvider);

            var user = usersRepository.Users.GetUser(idUser);
            var userGroupIds = user.UserGroups.Select(t => t.Id);
            var constant = _permissionConstants.Single(t => t.EntityType == entityType && t.CrudOperation == crudOperation);
            var result = await permissionProvider.GetUserGroupIdsWithPermissionAsync(userGroupIds, constant.Constant).ConfigureAwait(false);
            return result.ToArray();
        }
    }
}
