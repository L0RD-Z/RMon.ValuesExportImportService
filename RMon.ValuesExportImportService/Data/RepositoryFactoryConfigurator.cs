using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RMon.Configuration.Options;
using RMon.Data.Provider.Esb.Backend;
using RMon.Data.Provider.Security;
using RMon.Security.Core;
using RMon.Security.Provider.Sql;

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


        public IPermissionProvider PermissionProviderCreate()
        {
            return new SqlPermissionProvider(_dbOptionsMonitor.CurrentValue.ConnectionString, _loggerFactory);
        }

        public RMon.Data.Provider.Security.IRepository UsersRepositoryCreate(IPermissionProvider permissionProvider)
        {
            return new SqlRepository(_dbOptionsMonitor.CurrentValue.ConnectionString, permissionProvider, null, _loggerFactory);
        }

        public RMon.Data.Provider.Esb.Backend.IRepository TaskRepositoryCreate()
        {
            return TasksRepositoryFactory.CreateRepository(_dbOptionsMonitor.CurrentValue.ConnectionString, _loggerFactory);
        }

        #endregion
    }
}
