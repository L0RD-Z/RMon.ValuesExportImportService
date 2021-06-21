using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RMon.Configuration.Options;
using RMon.Context.FrontEndContext;
using RMon.Core.Base;

namespace RMon.ValuesExportImportService.Data
{
    public class FrontEndContextFactory : ISimpleFactory<FrontEndContext>
    {
        private readonly IOptionsMonitor<EntitiesDatabase> _dbOptionsMonitor;

        public FrontEndContextFactory(IOptionsMonitor<EntitiesDatabase> dbOptionsMonitor)
        {
            _dbOptionsMonitor = dbOptionsMonitor;
        }


        /// <summary>
        /// Создаёт и возвращает <see cref="FrontEndContext"/>
        /// </summary>
        /// <returns></returns>
        private FrontEndContext FrontEndContextCreate()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder().UseSqlServer(_dbOptionsMonitor.CurrentValue.ConnectionString);
            return new FrontEndContext(dbContextOptionsBuilder.Options, _dbOptionsMonitor.CurrentValue.ConnectionString);
        }

        #region ISimpleFactory<FrontEndContext>

        public FrontEndContext Create() => FrontEndContextCreate();

        #endregion
    }
}
