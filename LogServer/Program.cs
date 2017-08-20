namespace LogServer
{
    using LogServer.Contexts.ConsoleLog;
    using LogServer.Contexts.Http;
    using LogServer.Contexts.Http.Commands;
    using Serviceable.Objects.Composition;
    using Serviceable.Objects.Remote.Composition;

    class Program
    {
        static void Main(string[] args)
        {
            var configuration = @"
{
    GraphVertices: [
        { TypeFullName:'" + typeof(OwinHttpContext).FullName + @"', Id:'server-context' },
        { TypeFullName:'" + typeof(ConsoleLogContext).FullName + @"', Id:'log-context', ParentId:'server-context' },
    ]
}
";

            var contextGraph = new ContextGraph();
            contextGraph.FromJson(configuration);
            contextGraph.Execute(new Run());
        }
    }
}
