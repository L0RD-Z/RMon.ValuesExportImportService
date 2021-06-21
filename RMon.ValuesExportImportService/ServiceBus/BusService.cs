using System;
using MassTransit;
using MassTransit.Saga;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RMon.Configuration.Options;
using RMon.ESB.Common;
using RMon.ValuesExportImportService.ServiceBus.Export;
using RMon.ValuesExportImportService.ServiceBus.Import;
using RMon.ValuesExportImportService.ServiceBus.Parse;

namespace RMon.ValuesExportImportService.ServiceBus
{
    class BusService:BaseBusService
    {
        private readonly IOptionsMonitor<TasksEsb> _esbOptionsMonitor;
        private readonly IBusProvider _busProvider;
        private readonly ILogger<BusService> _logger;

        private readonly ExportStateMachine _exportStateMachine;
        private readonly ISagaRepository<ExportStateMachineInstance> _exportSagaRepository = new InMemorySagaRepository<ExportStateMachineInstance>();

        private readonly ParseStateMachine _parseStateMachine;
        private readonly ISagaRepository<ParseStateMachineInstance> _parseSagaRepository = new InMemorySagaRepository<ParseStateMachineInstance>();   
        
        private readonly ImportStateMachine _importStateMachine;
        private readonly ISagaRepository<ImportStateMachineInstance> _importSagaRepository = new InMemorySagaRepository<ImportStateMachineInstance>();




        public BusService(IOptionsMonitor<TasksEsb> esbOptionsMonitor, IBusProvider busProvider, ILogger<BusService> logger, ExportStateMachine exportStateMachine, ParseStateMachine parseStateMachine, ImportStateMachine importStateMachine)
        {
            _logger = logger;
            _busProvider = busProvider;
            _esbOptionsMonitor = esbOptionsMonitor;
            _exportStateMachine = exportStateMachine;
            _importStateMachine = importStateMachine;
            _parseStateMachine = parseStateMachine;

            SetEsbOptions(_esbOptionsMonitor.CurrentValue.Host, _esbOptionsMonitor.CurrentValue.Login, _esbOptionsMonitor.CurrentValue.Password);
            _esbOptionsMonitor.OnChange(EsbOptionsOnChange);
        }

        private void EsbOptionsOnChange(TasksEsb tasksEsbOptions)
        {
            SetEsbOptions(tasksEsbOptions.Host, tasksEsbOptions.Login, tasksEsbOptions.Password);
        }
        protected override void InitializeReceiveEndpoint(IReceiveEndpointConfigurator cfg)
        {
            cfg.StateMachineSaga(_exportStateMachine, _exportSagaRepository);
            cfg.StateMachineSaga(_parseStateMachine, _parseSagaRepository);
            cfg.StateMachineSaga(_importStateMachine, _importSagaRepository);
            base.InitializeReceiveEndpoint(cfg);
        }

        protected override void OnConfigurationChanged()
        {
            _logger.LogInformation($"Шина межсервисного взаимодействия переконфигурирована. Конфигурация: {_esbOptionsMonitor.CurrentValue}.");
            base.OnConfigurationChanged();
        }

        protected override void OnBadConfiguration()
        {
            _logger.LogWarning($"Неправильная конфигурация шины межсервисного взаимодействия. Ожидание правильной конфигурации для запуска шины. Конфигурация: {_esbOptionsMonitor.CurrentValue}.");
            base.OnBadConfiguration();
        }

        protected override void BusControlCreated(IBusControl busControl)
        {
            _busProvider.Bus = busControl;
            base.BusControlCreated(busControl);
        }

        protected override void OnConnecting()
        {
            _logger.LogInformation("Подключение к шине межсервисного взаимодействия...");
            base.OnConnecting();
        }

        protected override void OnConnected()
        {
            _logger.LogInformation("Шина межсервисного взаимодействия подключена.");
            base.OnConnected();
        }

        protected override void OnDisconnected()
        {
            _logger.LogInformation("Шина межсервисного взаимодействия отключена.");
            base.OnDisconnected();
        }

        protected override void OnException(Exception e, string message)
        {
            _logger.LogError(e, "Во время подключения к шине межсервисного взаимодействия возникло исключение.");
            base.OnException(e, message);
        }
    }
}
