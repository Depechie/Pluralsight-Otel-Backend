using Catalog.API.Extensions;
using Catalog.API.Models;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace Catalog.API;

public class Program
{
    public static ConfigurationManager Configuration { get; private set; }

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        Configuration = builder.Configuration;
        builder.Services.AddCORSPolicy(builder.Configuration);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHealthChecks();

        #region Serilog
        builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(hostingContext.Configuration)
            .WriteTo.OpenTelemetry(options =>
            {
                options.Endpoint = $"{Configuration.GetValue<string>("Otlp:Endpoint")}/v1/logs";
                options.Protocol = Serilog.Sinks.OpenTelemetry.OtlpProtocol.GrpcProtobuf;
                options.ResourceAttributes = new Dictionary<string, object>
                {
                    ["service.name"] = Configuration.GetValue<string>("Otlp:ServiceName")
                };
            }));
        #endregion

        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        #region OpenTelemetry
        Action<ResourceBuilder> appResourceBuilder =
            resource => resource
                .AddTelemetrySdk()
                .AddService(Configuration.GetValue<string>("Otlp:ServiceName"));

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(appResourceBuilder)
            .WithTracing(builder => builder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSource("APITracing")
                //.AddConsoleExporter()
                .AddOtlpExporter(options => options.Endpoint = new Uri(Configuration.GetValue<string>("Otlp:Endpoint")))
            )
            .WithMetrics(builder => builder
                .AddRuntimeInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(options => options.Endpoint = new Uri(Configuration.GetValue<string>("Otlp:Endpoint"))));
        #endregion

        var app = builder.Build();

        #region Swagger
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        #endregion

        //app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors(Policies.CORS_MAIN);
        app.UseAuthorization();
        app.UseSerilogRequestLogging();
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            AllowCachingResponses = false,
            ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                }
        });
        app.MapControllers();

        app.Run();
    }
}