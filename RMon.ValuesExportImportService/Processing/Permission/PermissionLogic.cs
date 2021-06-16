using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RMon.Commissioning.Core;
using RMon.ValuesExportImportService.Data;

namespace RMon.ValuesExportImportService.Processing.Permission
{
    public class PermissionLogic : IPermissionLogic
    {
        private readonly IRepositoryFactoryConfigurator _repositoryFactoryConfigurator;

        private readonly List<PermissionConstant> _permissionConstants = new()
        {
            new PermissionConstant(EntityTypes.Objects, CrudOperations.Create, "OE_CREATE_OBJECT"),
            new PermissionConstant(EntityTypes.Objects, CrudOperations.Read, "OC_VIEW_OBJECT_PROPERTIES"),
            new PermissionConstant(EntityTypes.Objects, CrudOperations.Update, "OE_EDIT_OBJECT"),
            new PermissionConstant(EntityTypes.Objects, CrudOperations.Delete, "OE_DELETE_OBJECT"),

            new PermissionConstant(EntityTypes.Devices, CrudOperations.Create, "OE_CREATE_DEVICE"),
            new PermissionConstant(EntityTypes.Devices, CrudOperations.Read, "OC_VIEW_DEVICE_PROPERTIES"),
            new PermissionConstant(EntityTypes.Devices, CrudOperations.Update, "OE_EDIT_DEVICE"),
            new PermissionConstant(EntityTypes.Devices, CrudOperations.Delete, "OE_DELETE_DEVICE"),

            new PermissionConstant(EntityTypes.LogicDevices, CrudOperations.Create, "OE_CREATE_EQUIPMENT"),
            new PermissionConstant(EntityTypes.LogicDevices, CrudOperations.Read, "OC_VIEW_EQUIPMENT_PROPERTIES"),
            new PermissionConstant(EntityTypes.LogicDevices, CrudOperations.Update, "OE_EDIT_EQUIPMENT"),
            new PermissionConstant(EntityTypes.LogicDevices, CrudOperations.Delete, "OE_DELETE_EQUIPMENT"),

            new PermissionConstant(EntityTypes.Hierarchy, CrudOperations.Create, "HH_NODE_CREATE"),
            new PermissionConstant(EntityTypes.Hierarchy, CrudOperations.Read, "HH_NODES_VIEW"),
            new PermissionConstant(EntityTypes.Hierarchy, CrudOperations.Update, "HH_NODE_EDIT"),
            new PermissionConstant(EntityTypes.Hierarchy, CrudOperations.Delete, "HH_NODE_DELETE"),

            new PermissionConstant(EntityTypes.Tags, CrudOperations.Create, "OE_EDIT_TAGS "),
            new PermissionConstant(EntityTypes.Tags, CrudOperations.Read, "OC_VIEW_EQUIPMENT_TAGS"),
            new PermissionConstant(EntityTypes.Tags, CrudOperations.Update, "OE_EDIT_TAGS"),
            new PermissionConstant(EntityTypes.Tags, CrudOperations.Delete, "OE_EDIT_TAGS"),

            new PermissionConstant(EntityTypes.DeviceQueries, CrudOperations.Create, "OE_EDIT_QUERY"),
            new PermissionConstant(EntityTypes.DeviceQueries, CrudOperations.Read, "OC_VIEW_DEVICE_PROPERTIES"),
            //new PermissionConstant(EntityTypes.DeviceQueries, CrudOperations.Update, "HH_NODE_EDIT"),
            new PermissionConstant(EntityTypes.DeviceQueries, CrudOperations.Delete, "OE_EDIT_QUERY "),
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
