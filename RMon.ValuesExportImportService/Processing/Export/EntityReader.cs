using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RMon.Commissioning.Core;
using RMon.Configuration.Options;
using RMon.Data.Provider;
using RMon.Data.Provider.Units.Backend.Common;
using RMon.Data.Provider.Units.Backend.Interfaces;
using RMon.Values.ExportImport.Core;
using RMon.ValuesExportImportService.Common;
using RMon.ValuesExportImportService.Data;
using RMon.ValuesExportImportService.Extensions;
using RMon.ValuesExportImportService.Processing.Permission;
using RMon.Data.Provider.Values;
using RMon.Globalization;
using RMon.Globalization.String;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Processing.Export
{
    class EntityReader
    {
        private readonly IOptionsMonitor<ValuesDatabase> _valuesDatabase;

        /// <summary>
        /// Репозиторий для работы с оборудованием
        /// </summary>
        private readonly ILogicDevicesRepository _logicDevicesRepository;
        
        private readonly IDataRepository _dataRepository;
        private readonly IPermissionLogic _permissionLogic;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="valuesDatabase"></param>
        /// <param name="logicDevicesRepository">Репозиторий доступа к данным</param>
        /// <param name="permissionLogic">Логика работы с правами доступа</param>
        /// <param name="dataRepository"></param>
        public EntityReader(IOptionsMonitor<ValuesDatabase> valuesDatabase, ILogicDevicesRepository logicDevicesRepository, IDataRepository dataRepository, IPermissionLogic permissionLogic)
        {
            _valuesDatabase = valuesDatabase;
            _logicDevicesRepository = logicDevicesRepository;
            _permissionLogic = permissionLogic;
            _dataRepository = dataRepository;
        }

        public async Task<ExportTable> Read(ValuesExportTaskParameters valuesExportTaskParameters, long idUser, CancellationToken cancellationToken = default)
        {
            var idUserGroups = await _permissionLogic.GetUserGroupIdsWithPermissionAsync(EntityTypes.LogicDevices, CrudOperations.Read, idUser).ConfigureAwait(false);

            var newPropertyCodes = valuesExportTaskParameters.PropertyCodes.ToList();
            newPropertyCodes.Add(PropertyCodes.Id);
            var entityDescription = newPropertyCodes.ToEntityDescription(EntityTypes.LogicDevices);
            var logicDevicesTable = await _logicDevicesRepository.GetLogicDevicesTable(idUserGroups, valuesExportTaskParameters.IdLogicDevices, entityDescription, cancellationToken).ConfigureAwait(false);

            var tags = await _dataRepository.GetTagsAsync(valuesExportTaskParameters.IdLogicDevices, valuesExportTaskParameters.TagTypeCodes).ConfigureAwait(false);

            var valuesRepository = ValueRepositoryFactory.GetRepository(
                _valuesDatabase.CurrentValue.IsMongo() ? EProviderEngine.Mongo : EProviderEngine.Sql,
                _valuesDatabase.CurrentValue.ConnectionString, _valuesDatabase.CurrentValue.ConnectionString);

            var timeRange = new TimeRange(valuesExportTaskParameters.DateTimeStart, valuesExportTaskParameters.DateTimeEnd);


            var entityDescriptions = new List<EntityDescription>()
            {
                logicDevicesTable.EntityDescription,
                new(EntityCodes.Tag, TextExcel.Tags, CreateTagPropertyDescriptions())
            };

            var entityTable = new EntityTable
            {
                EntityDescription = new EntityDescription(EntityCodes.Result, TextExcel.Results, CreateValuesPropertyDescriptions(), entityDescriptions),
            };

            foreach (var logicDevice in logicDevicesTable.Entities)
            {
                var idLogicDevice = long.Parse(logicDevice.Properties[PropertyCodes.Id].Value);
                var tagsLogicDevice = tags.Where(t => t.IdLogicDevice == idLogicDevice).ToList();
                foreach (var tag in tagsLogicDevice)
                {
                    var tagValues = await valuesRepository.GetValuesAsync(new []{ tag.Id }, timeRange).ConfigureAwait(false);
                    tagValues = tagValues.OrderBy(t => t.Datetime).ToList();
                    foreach (var value in tagValues)
                    {
                        var valuePropertyValues = CreateValuesPropertyValues(value.Datetime, value.ValueFloat ?? 0);
                        var tagPropertyValues = CreateTagPropertyValues(tag.Id, tag.LogicTagLink.LogicTagType.Code, tag.LogicTagLink.LogicTagType.Name);
                        var entity = new Entity(EntityCodes.Result, TextExcel.Results, valuePropertyValues, new List<Entity>
                        {
                            logicDevice,
                            new(EntityCodes.Tag, TextExcel.Tags, tagPropertyValues)
                        });
                        entityTable.Entities.Add(entity);
                    }
                }
            }

            


            return new ExportTable
            {
                PropertyCodes = CreateTagPropertyDescriptions().Select(t => t.Code).ToList(),
                EntityTable = entityTable
            };
        }

        List<PropertyDescription> CreateTagPropertyDescriptions() =>
            new()
            {
                new(PropertyCodes.Id, TextExcel.Id),
                new(PropertyCodes.Code, TextExcel.Code),
                new(PropertyCodes.Name, TextExcel.Name)
            };

        List<PropertyDescription> CreateValuesPropertyDescriptions() =>
            new()
            {
                new(PropertyCodes.Timestamp, TextExcel.Timestamp),
                new(PropertyCodes.Value, TextExcel.Value),
            };

        List<PropertyValue> CreateTagPropertyValues(long tagId, string tagCode, string tagName) =>
            new()
            {
                new(PropertyCodes.Id, tagId.ToString()),
                new(PropertyCodes.Code, tagCode),
                new(PropertyCodes.Name, tagName),
            };

        List<PropertyValue> CreateValuesPropertyValues(DateTime timestamp, double value) =>
            new()
            {
                new(PropertyCodes.Timestamp, timestamp.ToString()),
                new(PropertyCodes.Value, value.ToString())
            };
    }
}
