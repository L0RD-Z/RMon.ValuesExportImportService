using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RMon.Core.Base;
using RMon.Data.Provider;
using RMon.Data.Provider.Units.Backend.Common;
using RMon.Data.Provider.Units.Backend.Interfaces;
using RMon.Values.ExportImport.Core;
using RMon.ValuesExportImportService.Common;
using RMon.ValuesExportImportService.Data;
using RMon.ValuesExportImportService.Extensions;
using RMon.ValuesExportImportService.Processing.Permission;
using RMon.Data.Provider.Values;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Processing.Export
{
    class EntityReader : IEntityReader
    {
        private readonly ISimpleFactory<IValueRepository> _valueRepositorySimpleFactory;

        private readonly ILogicDevicesRepository _logicDevicesRepository;
        private readonly IDataRepository _dataRepository;
        private readonly IPermissionLogic _permissionLogic;


        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="valueRepositorySimpleFactory">Фабрика для создания репозитория значений</param>
        /// <param name="logicDevicesRepository">Репозиторий доступа к данным</param>
        /// <param name="permissionLogic">Логика работы с правами доступа</param>
        /// <param name="dataRepository"></param>
        public EntityReader(ISimpleFactory<IValueRepository> valueRepositorySimpleFactory, ILogicDevicesRepository logicDevicesRepository, IDataRepository dataRepository, IPermissionLogic permissionLogic)
        {
            _valueRepositorySimpleFactory = valueRepositorySimpleFactory;
            _logicDevicesRepository = logicDevicesRepository;
            _permissionLogic = permissionLogic;
            _dataRepository = dataRepository;
        }

        public async Task<ExportTable> Read(ValuesExportTaskParameters valuesExportTaskParameters, long idUser, CancellationToken cancellationToken = default)
        {
            var idUserGroups = await _permissionLogic.GetUserGroupIdsWithPermissionAsync(EntityTypes.Values, CrudOperations.Read, idUser).ConfigureAwait(false);

            var newPropertyCodes = valuesExportTaskParameters.PropertyCodes.ToList();
            newPropertyCodes.Add(LogicDevicePropertyCodes.Id);
            var entityDescription = newPropertyCodes.ToEntityDescription("LogicDevices");
            var logicDevicesTable = await _logicDevicesRepository.GetLogicDevicesTable(idUserGroups, valuesExportTaskParameters.IdLogicDevices, entityDescription, cancellationToken).ConfigureAwait(false);

            var tags = await _dataRepository.GetTagsAsync(valuesExportTaskParameters.IdLogicDevices, valuesExportTaskParameters.TagTypeCodes, cancellationToken).ConfigureAwait(false);

            var valuesRepository = _valueRepositorySimpleFactory.Create();

            var timeRange = new TimeRange(valuesExportTaskParameters.DateTimeStart, valuesExportTaskParameters.DateTimeEnd);
            var values = await valuesRepository.GetValuesAsync(tags.Select(t => t.Id).ToList(), timeRange).ConfigureAwait(false);

            var entityTable = new EntityTable
            {
                EntityDescription = new EntityDescription(EntityCodes.Result, TextExcel.Results, CreateValuesPropertyDescriptions(), CreateEntityDescription(logicDevicesTable.EntityDescription))
            };

            foreach (var logicDevice in logicDevicesTable.Entities)
            {
                var idLogicDevice = long.Parse(logicDevice.Properties[LogicDevicePropertyCodes.Id].Value);
                var tagsLogicDevice = tags.Where(t => t.IdLogicDevice == idLogicDevice).ToList();
                foreach (var tag in tagsLogicDevice)
                {
                    var tagValues = values.Where(t => t.IdDeviceTag == tag.Id).OrderBy(t => t.Datetime).ToList();
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

        List<EntityDescription> CreateEntityDescription(EntityDescription logicDeviceEntityDescription) =>
            new()
            {
                logicDeviceEntityDescription,
                new(EntityCodes.Tag, TextExcel.Tags, CreateTagPropertyDescriptions())
            };


        List<PropertyDescription> CreateTagPropertyDescriptions() =>
            new()
            {
                new(TagPropertyCodes.Id, TextExcel.Id),
                new(TagPropertyCodes.Code, TextExcel.Code),
                new(TagPropertyCodes.Name, TextExcel.Name)
            };

        List<PropertyDescription> CreateValuesPropertyDescriptions() =>
            new()
            {
                new(ValuesPropertyCodes.Timestamp, TextExcel.Timestamp),
                new(ValuesPropertyCodes.Value, TextExcel.Value),
            };

        List<PropertyValue> CreateTagPropertyValues(long tagId, string tagCode, string tagName) =>
            new()
            {
                new(TagPropertyCodes.Id, tagId.ToString()),
                new(TagPropertyCodes.Code, tagCode),
                new(TagPropertyCodes.Name, tagName),
            };

        List<PropertyValue> CreateValuesPropertyValues(DateTime timestamp, double value) =>
            new()
            {
                new(ValuesPropertyCodes.Timestamp, timestamp.ToString()),
                new(ValuesPropertyCodes.Value, value.ToString())
            };
    }
}
