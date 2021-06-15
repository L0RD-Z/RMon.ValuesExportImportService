using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RMon.Configuration.Options;
using RMon.Context.FrontEndContext;
using RMon.Core.Base;

namespace RMon.ValuesExportImportService.Data
{
    public class FrontEndContextFactory:ISimpleFactory<FrontEndContext>
    {
        private readonly IOptionsMonitor<EntitiesDatabase> _dbOptionsMonitor;
        protected readonly ILoggerFactory LoggerFactory;


        /// <summary>
        /// Нужен для автотестов
        /// </summary>
        protected FrontEndContextFactory()
            :this(null, null)
        {
            //LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(c => c.AddDebug());
        }

        public FrontEndContextFactory(IOptionsMonitor<EntitiesDatabase> dbOptionsMonitor)
            : this(dbOptionsMonitor, null)
        {
        }

        public FrontEndContextFactory(IOptionsMonitor<EntitiesDatabase> dbOptionsMonitor, ILoggerFactory loggerFactory)
        {
            _dbOptionsMonitor = dbOptionsMonitor;
            //LoggerFactory = loggerFactory;
        }


        /// <summary>
        /// Создаёт и возвращает <see cref="FrontEndContext"/>
        /// </summary>
        /// <returns></returns>
        protected virtual FrontEndContext FrontEndContextCreate()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder().UseSqlServer(_dbOptionsMonitor.CurrentValue.ConnectionString);
            if (LoggerFactory != null)
                dbContextOptionsBuilder = dbContextOptionsBuilder.UseLoggerFactory(LoggerFactory);

            return new FrontEndContext(dbContextOptionsBuilder.Options, _dbOptionsMonitor.CurrentValue.ConnectionString);
        }

        #region ISimpleFactory<FrontEndContext>

        public FrontEndContext Create() => FrontEndContextCreate();

        #endregion
    }
}
