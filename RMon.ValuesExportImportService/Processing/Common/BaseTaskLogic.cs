using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RMon.Configuration.Options;
using RMon.ESB.Core.Common;
using RMon.ValuesExportImportService.Data;
using RMon.ValuesExportImportService.Excel;
using RMon.ValuesExportImportService.Files;
using RMon.ValuesExportImportService.Globalization;
using RMon.ValuesExportImportService.Processing.Permission;
using RMon.ValuesExportImportService.ServiceBus;

namespace RMon.ValuesExportImportService.Processing.Common
{
    abstract class BaseTaskLogic : ITaskLogic
    {
        protected readonly ILogger Logger;
        protected readonly IOptionsMonitor<Service> ServiceOptions;
        protected readonly IRepositoryFactoryConfigurator TaskFactoryRepositoryConfigurator;
        protected IDataRepository DataRepository { get; }
        protected readonly IPermissionLogic PermissionLogic;
        protected readonly IFileStorage FileStorage;
        
        protected readonly IGlobalizationProviderFactory GlobalizationProviderFactory;
        protected readonly ILanguageRepository LanguageRepository;

        /// <summary>
        /// Конструктор 1
        /// </summary>
        /// <param name="logger">Логгер</param>
        /// <param name="serviceOptions">Опции сервиса</param>
        /// <param name="taskFactoryRepositoryConfigurator">Конфигуратор репозиторияя для логирования хода выполнения задач</param>
        /// <param name="dataRepository">Репозиторий данных</param>
        /// <param name="permissionLogic">Логика работы с прадвами доступа</param>
        /// <param name="fileStorage">Файловое хранилище</param>
        /// <param name="globalizationProviderFactory"></param>
        /// <param name="languageRepository"></param>
        protected BaseTaskLogic(ILogger logger, 
            IOptionsMonitor<Service> serviceOptions, 
            IRepositoryFactoryConfigurator taskFactoryRepositoryConfigurator, 
            IDataRepository dataRepository,
            IPermissionLogic permissionLogic,
            IFileStorage fileStorage, 
            IGlobalizationProviderFactory globalizationProviderFactory,
            ILanguageRepository languageRepository)
        {
            Logger = logger;
            ServiceOptions = serviceOptions;
            TaskFactoryRepositoryConfigurator = taskFactoryRepositoryConfigurator;
            DataRepository = dataRepository;
            PermissionLogic = permissionLogic;
            FileStorage = fileStorage;
            GlobalizationProviderFactory = globalizationProviderFactory;
            LanguageRepository = languageRepository;
        }

        /// <inheritdoc/>
        public abstract Task StartTaskAsync(ITask receivedTask, CancellationToken ct = default);

        /// <inheritdoc/>
        public abstract void AbortTask(ITask receivedTask, StateMachineInstance instance);
    }
}
