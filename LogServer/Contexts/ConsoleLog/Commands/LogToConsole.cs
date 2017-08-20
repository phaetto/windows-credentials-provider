namespace LogServer.Contexts.ConsoleLog.Commands
{
    using System;
    using LogServer.Contexts.ConsoleLog.Commands.Data;
    using Serviceable.Objects.Remote;

    public sealed class LogToConsole : ReproducibleCommandWithData<ConsoleLogContext, ConsoleLogContext, LogData>
    {
        public LogToConsole(LogData data) : base(data)
        {
        }

        public override ConsoleLogContext Execute(ConsoleLogContext context)
        {
            Console.WriteLine($"{DateTime.Now}: {Data.Text}");
            return context;
        }
    }
}
