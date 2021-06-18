using System;
using System.Threading;
using Automatonymous;

namespace RMon.ValuesExportImportService.ServiceBus
{
    abstract class StateMachineInstance : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }

        public string CurrentState { get; set; }

        /// <summary>
        /// Токен для возможности отмены задачи
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; set; }



        protected StateMachineInstance() => CancellationTokenSource = new CancellationTokenSource();
    }
}
