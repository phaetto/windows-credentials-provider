namespace LogServer.Contexts.Http
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Serviceable.Objects;
    using Serviceable.Objects.Composition.Events;
    using Serviceable.Objects.Remote.Serialization;

    public sealed class OwinHttpContext : Context<OwinHttpContext>
    {
        public readonly IWebHost Host;

        public OwinHttpContext()
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables().Build();

            Host = new WebHostBuilder()
                .UseKestrel()
                .UseConfiguration(config)
                .UseContentRoot(Directory.GetCurrentDirectory())
                // .ConfigureLogging(l => l.AddConsole())
                .ConfigureServices(s => s.AddRouting())
                .Configure(app =>
                {
                    app.UseRouter(SetupRouter);
                })
                .UseUrls("http://localhost:5050")
                .Build();
        }

        private void SetupRouter(IRouteBuilder routerBuilder)
        {
            routerBuilder.MapPost("log", TestRequestHandler);
        }

        private async Task TestRequestHandler(HttpContext context)
        {
            string data;
            using (var streamReader = new StreamReader(context.Request.Body))
            {
                data = streamReader.ReadToEnd();
            }

            var spec = DeserializableSpecification<ExecutableCommandSpecification>.DeserializeFromJson(data);
            var command = spec.CreateFromSpec();
            
            var eventResults =
                OnCommandEventWithResultPublished(new GraphFlowEventPushControlApplyCommandInsteadOfEvent(command))
                .Where(x => x.ResultObject != null).ToList();

            if (eventResults.Count > 0)
            {
                var results = eventResults.Select(x => x.ResultObject);
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(results));
            }
            else
            {
                context.Response.StatusCode = 204;
            }
        }
    }
}
