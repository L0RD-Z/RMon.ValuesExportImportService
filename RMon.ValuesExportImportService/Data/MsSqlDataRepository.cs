using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RMon.Context.BackEndContext;
using RMon.Context.EntityStore;
using RMon.Core.Base;
using RMon.Data.Provider.Units.Backend.Common;
using RMon.ValuesExportImportService.Common;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Data
{
    class MsSqlDataRepository : IDataRepository
    {
        private readonly ISimpleFactory<BackEndContext> _factory;

        public MsSqlDataRepository(ISimpleFactory<BackEndContext> factory)
        {
            _factory = factory;
        }


        /// <inheritdoc/>
        public async Task<DateTime> GetDateAsync()
        {
            await using var dataContext = _factory.Create();
            return await dataContext.GetServerDateTimeAsync().ConfigureAwait(false);
        }

        #region Экспорт

        /// <inheritdoc/>
        public async Task<IList<Tag>> GetTagsAsync(IList<long> idLogicDevices, IList<string> tagCodes, CancellationToken cancellationToken)
        {
            await using var dataContext = _factory.Create();
            return await dataContext.Tags.AsNoTracking()
                .Include(t => t.LogicTagLink)
                .ThenInclude(t => t.LogicTagType)
                .Where(t => idLogicDevices.Contains(t.IdLogicDevice) && tagCodes.Contains(t.LogicTagLink.LogicTagType.Code))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        #endregion

        #region Парсинг

        #region 80020

        /// <inheritdoc/>
        public async Task<LogicDevice> GetLogicDeviceByPropertyValueAsync(string propertyCode, string propertyValue, CancellationToken cancellationToken)
        {
            await using var dataContext = _factory.Create();
            var logicDevices = await dataContext.LogicDevices.AsNoTracking()
                .Include(t => t.Tags)
                .ThenInclude(t => t.LogicTagLink)
                .ThenInclude(t => t.LogicTagType)
                .Include(t => t.Tags)
                .ThenInclude(t => t.DeviceTag)
                .Where(t => t.LogicDeviceProperties.Any(tt => tt.LogicDevicePropertyType.Code == propertyCode && tt.Value == propertyValue))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return logicDevices.Count switch
            {
                0 => throw new DataLayerException(TextDb.SelectedNoOneLogicDeviceError.With(propertyCode, propertyValue)),
                1 => logicDevices.Single(),
                _ => throw new DataLayerException(TextDb.SelectedManyLogicDeviceError.With(propertyCode, propertyValue))
            };
        }


        #endregion


        #region Flexiable

        /// <inheritdoc/>
        public async Task<IList<long>> FindTags(IList<long> idUserGroups, long idLogicDevice, Entity entityFilter, CancellationToken ct = default)
        {
            await using var context = _factory.Create();

            IQueryable<Tag> tags = context.Tags
                .Where(t => t.LogicDevice.Id == idLogicDevice && t.LogicDevice.UserGroupLogicDevices.Any(ugs => idUserGroups.Contains(ugs.IdUserGroup)));

            
            foreach (var property in entityFilter.Properties) 
                tags = AddTagPropertyCondition(tags, property.Value);


            return await tags.AsNoTracking()
                .Select(t => t.Id)
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }

        private IQueryable<Tag> AddTagPropertyCondition(IQueryable<Tag> queryable, PropertyValue propertyValue) =>
            propertyValue.Code switch
            {
                TagPropertyCodes.Id => long.TryParse(propertyValue.Value, out var lValue)
                    ? queryable.Where(t => t.Id == lValue)
                    : queryable,
                TagPropertyCodes.Code => !string.IsNullOrEmpty(propertyValue.Value)
                    ? queryable.Where(t => t.LogicTagLink.LogicTagType.Code == propertyValue.Value)
                    : queryable,
                TagPropertyCodes.Name => !string.IsNullOrEmpty(propertyValue.Value)
                    ? queryable.Where(t => t.LogicTagLink.LogicTagType.Name == propertyValue.Value)
                    : queryable,
                _ => queryable
            };

        #endregion


        #endregion

    }
}
