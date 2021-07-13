using System.Collections.Generic;
using System.Linq;
using Automatonymous;
using Microsoft.Extensions.Logging;
using RMon.ESB.Core.ValuesImportTaskDto;
using RMon.Values.ExportImport.Core;
using RMon.ValuesExportImportService.Processing;
using RMon.ValuesExportImportService.Processing.Import;
using RMon.ValuesExportImportService.ServiceBus.Common;

namespace RMon.ValuesExportImportService.ServiceBus.Import
{
    class ImportStateMachine : MassTransitStateMachine<ImportStateMachineInstance>
    {
        private readonly ILogger<BusLogger> _busLogger;
        private readonly ITaskLogic _taskLogic;


        public Event<IValuesImportTask> TaskReceived { get; set; }
        public Event<IValuesImportTaskFinished> TaskFinished { get; set; }
        public Event<IValuesImportTaskAbort> TaskAbort { get; set; }

        public State Executing { get; set; }



        public ImportStateMachine(ILogger<BusLogger> logger, IImportTaskLogic taskLogic)
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

        void Start(BehaviorContext<ImportStateMachineInstance, IValuesImportTask> context)
        {
            if (context.Data?.Parameters?.Values != null)
            {
                var values = context.Data.Parameters.Values.Select(t => t).ToList();
                context.Data.Parameters.Values.Clear();
                _busLogger.LogReceivedTask(context.Data, typeof(IValuesImportTask));
                context.Data.Parameters.Values = values.Select(t => t).ToList();
            }
            else
                _busLogger.LogReceivedTask(context.Data, typeof(IValuesImportTask));

            _taskLogic.StartTaskAsync(context.Data, context.Instance.CancellationTokenSource.Token);
        }

        void Abort(BehaviorContext<ImportStateMachineInstance, IValuesImportTaskAbort> context)
        {
            _busLogger.LogReceivedTask(context.Data, typeof(IValuesImportTaskAbort));
            _taskLogic.AbortTask(context.Data, context.Instance);
        }

        void FinishTask(BehaviorContext<ImportStateMachineInstance, IValuesImportTaskFinished> context)
        {
        }
    }
}
