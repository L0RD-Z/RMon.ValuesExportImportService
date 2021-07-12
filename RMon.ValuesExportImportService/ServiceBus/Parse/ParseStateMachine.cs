using Automatonymous;
using Microsoft.Extensions.Logging;
using RMon.ESB.Core.ValuesParseTaskDto;
using RMon.ValuesExportImportService.Processing;
using RMon.ValuesExportImportService.Processing.Parse;
using RMon.ValuesExportImportService.ServiceBus.Common;

namespace RMon.ValuesExportImportService.ServiceBus.Parse
{
    class ParseStateMachine : MassTransitStateMachine<ParseStateMachineInstance>
    {
        private readonly ILogger<BusLogger> _busLogger;
        private readonly ITaskLogic _taskLogic;


        public Event<IValuesParseTask> TaskReceived { get; set; }
        public Event<IValuesParseTaskFinished> TaskFinished { get; set; }
        public Event<IValuesParseTaskAbort> TaskAbort { get; set; }

        public State Executing { get; set; }



        public ParseStateMachine(ILogger<BusLogger> logger, IParseTaskLogic taskLogic)
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

        void Start(BehaviorContext<ParseStateMachineInstance, IValuesParseTask> context)
        {
            _busLogger.LogReceivedTask(context.Data, typeof(IValuesParseTask));
            _taskLogic.StartTaskAsync(context.Data, context.Instance.CancellationTokenSource.Token);
        }

        void Abort(BehaviorContext<ParseStateMachineInstance, IValuesParseTaskAbort> context)
        {
            _busLogger.LogReceivedTask(context.Data, typeof(IValuesParseTaskAbort));
            _taskLogic.AbortTask(context.Data, context.Instance);
        }

        void FinishTask(BehaviorContext<ParseStateMachineInstance, IValuesParseTaskFinished> context)
        {
            
        }
    }
}
