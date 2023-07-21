using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using ServiceWorker.Services;
using ServiceWorker.Services.Interfaces;

namespace ServiceWorker
{
    internal class Program
    {
        private static IConfiguration Configuration { get; set; }
        
        static void Main(string[] args)
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .WriteTo.OpenTelemetry(options =>
                {
                    options.Endpoint = $"{Configuration.GetValue<string>("Otlp:Endpoint")}/v1/logs";
                    options.Protocol = Serilog.Sinks.OpenTelemetry.OtlpProtocol.GrpcProtobuf;
                    options.ResourceAttributes = new Dictionary<string, object>
                    {
                        ["service.name"] = Configuration.GetValue<string>("Otlp:ServiceName")
                    };
                })
                .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    Action<ResourceBuilder> appResourceBuilder =
                        resource => resource
                            .AddTelemetrySdk()
                            .AddService(Configuration.GetValue<string>("Otlp:ServiceName"));

                    services.AddOpenTelemetry()
                        .ConfigureResource(appResourceBuilder)
                        .WithTracing(builder => builder
                            .AddSource("APITracing")
                            //.AddConsoleExporter()
                            .AddOtlpExporter(options => options.Endpoint = new Uri(Configuration.GetValue<string>("Otlp:Endpoint")))
                        );

                    services.AddHttpClient<ICatalogService, CatalogService>();
                    services.AddHostedService<Worker>();
                })
            .UseSerilog();
    }
}