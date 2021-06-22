using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RMon.Context.BackEndContext;
using RMon.Context.EntityStore;
using RMon.Core.Base;
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
                .Include(t => t.Tags)
                .ThenInclude(t => t.DeviceTag)
                .Where(t => t.LogicDeviceProperties.Any(tt => tt.LogicDevicePropertyType.Code == propertyCode && tt.Value == propertyValue))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return logicDevices.Count switch
            {
                0 => throw new DataLayerException(TextDb.SelectedNoOneLogicDeviceError.With(propertyCode, propertyCode)),
                1 => logicDevices.Single(),
                _ => throw new DataLayerException(TextDb.SelectedManyLogicDeviceError.With(logicDevices.Count, propertyCode, propertyCode))
            };
        }


        #endregion



        #endregion

    }
}
