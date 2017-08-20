namespace LogServer.Contexts.Http.Commands
{
    using Microsoft.AspNetCore.Hosting;
    using Serviceable.Objects;

    public sealed class Run : ICommand<OwinHttpContext, OwinHttpContext>
    {
        public OwinHttpContext Execute(OwinHttpContext context)
        {
            context.Host.Run();
            return context;
        }
    }
}
