using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RMon.Context.BackEndContext;
using RMon.Context.EntityStore;
using RMon.Core.Base;

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

        public async Task<IList<Tag>> GetTagsAsync(IList<long> idLogicDevices, IList<string> tagCodes)
        {
            await using var dataContext = _factory.Create();
            return await dataContext.Tags.AsNoTracking()
                .Include(t => t.LogicTagLink)
                .ThenInclude(t => t.LogicTagType)
                .Where(t => idLogicDevices.Contains(t.IdLogicDevice) && tagCodes.Contains(t.LogicTagLink.LogicTagType.Code))
                .ToListAsync()
                .ConfigureAwait(false);
        }

    }
}
