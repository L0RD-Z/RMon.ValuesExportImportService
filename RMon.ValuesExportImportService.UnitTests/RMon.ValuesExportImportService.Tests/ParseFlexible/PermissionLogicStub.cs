using System.Threading.Tasks;
using RMon.ValuesExportImportService.Processing.Permission;

namespace RMon.ValuesExportImportService.Tests.ParseFlexible
{
    class PermissionLogicStub : IPermissionLogic
    {
        public Task<long[]> GetUserGroupIdsWithPermissionAsync(EntityTypes entityType, CrudOperations crudOperation, long idUser) =>
            Task.FromResult(new long[]
            {
                25647, 52345
            });
    }
}
