using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RMon.Configuration.Options;
using RMon.Context.BackEndContext;
using RMon.Context.EntityStore;
using RMon.Core.Base;

namespace RMon.ValuesExportImportService.Data
{
    class MsSqlDataRepository : IDataRepository, ISimpleFactory<BackEndContext>
    {
        private readonly IOptionsMonitor<EntitiesDatabase> _dbOptionsMonitor;


        public MsSqlDataRepository(IOptionsMonitor<EntitiesDatabase> dbOptionsMonitor)
        {
            _dbOptionsMonitor = dbOptionsMonitor;
        }

        /// <summary>
        /// Создаёт и возвращает <see cref="BackEndContext"/>
        /// </summary>
        /// <returns></returns>
        private BackEndContext BackEndContextCreate()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder().UseSqlServer(_dbOptionsMonitor.CurrentValue.ConnectionString);
            return new BackEndContext(dbContextOptionsBuilder.Options);
        }


        /// <inheritdoc/>
        public async Task<DateTime> GetDateAsync()
        {
            await using var dataContext = BackEndContextCreate();
            return await dataContext.GetServerDateTimeAsync().ConfigureAwait(false);
        }

        public async Task<IList<Tag>> GetTagsAsync(IList<long> idLogicDevices, IList<string> tagCodes)
        {
            await using var dataContext = BackEndContextCreate();
            return await dataContext.Tags.AsNoTracking()
                .Include(t => t.LogicTagLink)
                .ThenInclude(t => t.LogicTagType)
                .Where(t => idLogicDevices.Contains(t.IdLogicDevice) && tagCodes.Contains(t.LogicTagLink.LogicTagType.Code))
                .ToListAsync()
                .ConfigureAwait(false);
        }

        #region ISimpleFactory<BackEndContext>

        public BackEndContext Create() => BackEndContextCreate();

        #endregion

    }
}
