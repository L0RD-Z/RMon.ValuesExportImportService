using Automatonymous;
using Microsoft.Extensions.Logging;
using RMon.ESB.Core.ValuesExportTaskDto;
using RMon.ValuesExportImportService.Processing;
using RMon.ValuesExportImportService.Processing.Export;
using RMon.ValuesExportImportService.ServiceBus.Common;

namespace RMon.ValuesExportImportService.ServiceBus.Export
{
    class ExportStateMachine: MassTransitStateMachine<ExportStateMachineInstance>
    {
        private readonly ILogger<BusLogger> _busLogger;
        private readonly ITaskLogic _taskLogic;

        
        public Event<IValuesExportTask> TaskReceived { get; set; }
        public Event<IValuesExportTaskFinished> TaskFinished { get; set; }
        public Event<IValuesExportTaskAbort> TaskAbort { get; set; }
        
        public State Executing { get; set; }

        


        public ExportStateMachine(ILogger<BusLogger> logger, IExportTaskLogic taskLogic)
        {
            _busLogger = logger;
            _taskLogic = taskLogic;
            InstanceState(t => t.CurrentState);
            
            Event(() => TaskReceived, t => t.CorrelateById(context => context.Message.CorrelationId));
            Event(() => TaskAbort, t => t.CorrelateById(context => context.Message.CorrelationId));
            Event(() => TaskFinished, t => t.CorrelateById(context => context.Message.CorrelationId));

            Initially(
                When(TaskReceived)
                .Then(Start)
                .TransitionTo(Executing));

            During(Executing, 
                When(TaskAbort)
                .Then(Abort)
                .Finalize());

            During(Executing, 
                When(TaskFinished)
                .Then(FinishTask)
                .Finalize());

            SetCompletedWhenFinalized();
        }

        void Start(BehaviorContext<ExportStateMachineInstance, IValuesExportTask> context)
        {
            _busLogger.LogReceivedTask(context.Data, typeof(IValuesExportTask));
            _taskLogic.StartTaskAsync(context.Data, context.Instance.CancellationTokenSource.Token);
        }

        void Abort(BehaviorContext<ExportStateMachineInstance, IValuesExportTaskAbort> context)
        {
            _busLogger.LogReceivedTask(context.Data, typeof(IValuesExportTaskAbort));
            _taskLogic.AbortTask(context.Data, context.Instance);
        }

        void FinishTask(BehaviorContext<ExportStateMachineInstance, IValuesExportTaskFinished> context)
        {
            _busLogger.LogReceivedTask(context.Data, typeof(IValuesExportTaskFinished));
        }
    }
}
