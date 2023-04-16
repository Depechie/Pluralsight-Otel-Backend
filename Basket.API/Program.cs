using Basket.API.Extensions;
using Basket.API.Models;
using Basket.API.Services;
using Basket.API.Services.Interfaces;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Basket.API;

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
        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        builder.Services.AddHttpClient<ICatalogService, CatalogService>();

        builder.Services.AddOpenTelemetry().WithTracing(builder =>
        {
            builder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(Configuration.GetValue<string>("Otlp:ServiceName")))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSource("APITracing")
                .AddConsoleExporter() // TODO: Glenn - Only add in app.Environment.IsDevelopment()
                .AddOtlpExporter(options => options.Endpoint = new Uri(Configuration.GetValue<string>("Otlp:Endpoint")));
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors(Policies.CORS_MAIN);
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                AllowCachingResponses = false,
                ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    }
            });
            endpoints.MapControllers();
        });

        app.MapControllers();

        app.Run();
    }
}

