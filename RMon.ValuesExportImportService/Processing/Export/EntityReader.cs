using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RMon.Commissioning.Core;
using RMon.Data.Provider.Units.Backend.Interfaces;
using RMon.ValuesExportImportService.Common;
using RMon.ValuesExportImportService.Extensions;
using RMon.ValuesExportImportService.Processing.Permission;

namespace RMon.ValuesExportImportService.Processing.Export
{
    class EntityReader
    {
        /// <summary>
        /// Репозиторий для работы с оборудованием
        /// </summary>
        private readonly ILogicDevicesRepository _logicDevicesRepository;
        /// <summary>
        /// Репозиторий для работы с Тегами
        /// </summary>
        private readonly ITagsRepository _tagsRepository;
        private readonly IPermissionLogic _permissionLogic;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="logicDevicesRepository">Репозиторий доступа к данным</param>
        /// <param name="permissionLogic">Логика работы с правами доступа</param>
        /// <param name="tagsRepository"></param>
        public EntityReader(ILogicDevicesRepository logicDevicesRepository, ITagsRepository tagsRepository, IPermissionLogic permissionLogic)
        {
            _logicDevicesRepository = logicDevicesRepository;
            _permissionLogic = permissionLogic;
            _tagsRepository = tagsRepository;
        }

        public async Task Read(IList<long> idLogicDevices, IList<string> propertyCodes, long idUser, CancellationToken cancellationToken = default)
        {
            var idUserGroupsLogicDevices = await _permissionLogic.GetUserGroupIdsWithPermissionAsync(EntityTypes.LogicDevices, CrudOperations.Read, idUser).ConfigureAwait(false);

            var entityDescription = propertyCodes.ToEntityDescription(EntityTypes.LogicDevices);
            //var logicDevicesTable = await _logicDevicesRepository.GetLogicDevicesTable(idUserGroupsLogicDevices, idLogicDevices, entityDescription, cancellationToken).ConfigureAwait(false);

            var idUserGroups = await _permissionLogic.GetUserGroupIdsWithPermissionAsync(EntityTypes.Tags, CrudOperations.Read, idUser).ConfigureAwait(false);

            // var entityDescription = propertyCodes.ToEntityDescription(EntityTypes.Tags);
            var tagsTable = await _tagsRepository.GetTagsTable(idUserGroups, idLogicDevices, entityDescription, cancellationToken).ConfigureAwait(false);
        }
    }
}
