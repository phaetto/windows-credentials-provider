namespace LogServer.Contexts.ConsoleLog
{
    using System;
    using System.Collections.Generic;
    using Serviceable.Objects;
    using Serviceable.Objects.Composition;

    public sealed class ConsoleLogContext: Context<ConsoleLogContext>, IPostGraphFlowPullControl
    {
        public void PullNodeExecutionInformation(ContextGraph contextGraph, string executingNodeId, dynamic parentContext,
            dynamic parentCommandApplied, Stack<EventResult> eventResults)
        {
            Console.WriteLine("\n\n*** Executed ***" +
                              $"\n\tNode '{executingNodeId}'," +
                              $"\n\tContext: '{((object)parentContext).GetType().FullName}'," +
                              $"\n\tCommand: '{((object)parentCommandApplied).GetType().FullName}'" +
                              "\n\n");
        }
    }
}
