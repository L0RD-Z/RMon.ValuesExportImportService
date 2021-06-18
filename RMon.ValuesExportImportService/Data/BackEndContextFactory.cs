using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RMon.Configuration.Options;
using RMon.Context.BackEndContext;
using RMon.Core.Base;

namespace RMon.ValuesExportImportService.Data
{
    class BackEndContextFactory : ISimpleFactory<BackEndContext>
    {
        private readonly IOptionsMonitor<EntitiesDatabase> _dbOptionsMonitor;

        public BackEndContextFactory(IOptionsMonitor<EntitiesDatabase> dbOptionsMonitor)
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



        #region ISimpleFactory<BackEndContext>

        public BackEndContext Create() => BackEndContextCreate();

        #endregion

    }
}
