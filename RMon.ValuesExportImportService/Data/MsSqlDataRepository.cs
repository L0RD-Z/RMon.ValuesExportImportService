using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RMon.Configuration.Options;
using RMon.Context.BackEndContext;

namespace RMon.ValuesExportImportService.Data
{
    class MsSqlDataRepository:IDataRepository
    {
        private readonly IOptionsMonitor<EntitiesDatabase> _dbOptionsMonitor;
        protected readonly ILoggerFactory LoggerFactory;

        /// <summary>
        /// Нужен для автотестов
        /// </summary>
        protected MsSqlDataRepository()
            :this(null, null)
        {
            // LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(c => c.AddDebug());
        }

        public MsSqlDataRepository(IOptionsMonitor<EntitiesDatabase> dbOptionsMonitor)
            : this(dbOptionsMonitor, null)
        {

        }

        public MsSqlDataRepository(IOptionsMonitor<EntitiesDatabase> dbOptionsMonitor, ILoggerFactory loggerFactory)
        {
            _dbOptionsMonitor = dbOptionsMonitor;
            //LoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Создаёт и возвращает <see cref="BackEndContext"/>
        /// </summary>
        /// <returns></returns>
        private BackEndContext BackEndContextCreate()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder().UseSqlServer(_dbOptionsMonitor.CurrentValue.ConnectionString);
            if (LoggerFactory != null)
                dbContextOptionsBuilder = dbContextOptionsBuilder.UseLoggerFactory(LoggerFactory);

            return new BackEndContext(dbContextOptionsBuilder.Options);
        }


        /// <inheritdoc/>
        public async Task<DateTime> GetDateAsync()
        {
            await using var dataContext = BackEndContextCreate();
            return await dataContext.GetServerDateTimeAsync().ConfigureAwait(false);
        }
    }
}
