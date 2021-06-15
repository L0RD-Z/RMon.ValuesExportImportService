using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RMon.Configuration.Options;
using RMon.Data.Provider.Esb.Backend;

namespace RMon.ValuesExportImportService.Data
{
    public class RepositoryFactoryConfigurator : IRepositoryFactoryConfigurator
    {
        private readonly IOptionsMonitor<EntitiesDatabase> _dbOptionsMonitor;
        private readonly ILoggerFactory _loggerFactory;

        public RepositoryFactoryConfigurator(IOptionsMonitor<EntitiesDatabase> dbOptionsMonitor)
        {
            _dbOptionsMonitor = dbOptionsMonitor;
            _loggerFactory = LoggerFactory.Create(c => c.AddDebug());
        }

        #region IRepositoryFactoryConfigurator


        public IRepository TaskRepositoryCreate()
        {
            return TasksRepositoryFactory.CreateRepository(_dbOptionsMonitor.CurrentValue.ConnectionString, _loggerFactory);
        }

        #endregion
    }
}
