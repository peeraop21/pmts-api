using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;

namespace PMTs.DataAccess.Tracing
{
    public static class OpenTelemetryExtensions
    {
        public static void AddOpenTelemetryTracing(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OpenTelemetryParameters>(configuration.GetSection("OpenTelemetry"));
            //services.Configure<AspNetCoreInstrumentationOptions>(configuration.GetSection("AspNetCoreInstrumentation"));

            var openTelemetryParameters = configuration.GetSection("OpenTelemetry").Get<OpenTelemetryParameters>();

            ActivitySourceProvider.Source = new System.Diagnostics.ActivitySource(openTelemetryParameters!.ActivitySourceName!);
            Action<ResourceBuilder> configureResource = r => r.AddService(serviceName: openTelemetryParameters!.ServiceName!, serviceVersion: openTelemetryParameters!.ServiceVersion!, serviceInstanceId: openTelemetryParameters!.ServiceInstanceId!);
            services.AddOpenTelemetry()
                .WithTracing(builder =>
                {
                    builder.ConfigureResource(configureResource)
                    .AddSource(openTelemetryParameters!.ActivitySourceName!)
                    .SetSampler(new AlwaysOnSampler()).AddSqlClientInstrumentation(options => options.SetDbStatementForText = true)
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation(o =>
                    {
                        o.Filter = (context) => !string.IsNullOrEmpty(context.Request.Path.Value) && context.Request.Path.Value.Contains("api", StringComparison.InvariantCulture);

                        // enrich activity with http request and response
                        o.EnrichWithHttpRequest = (activity, httpRequest) => { activity.SetTag("requestProtocol", httpRequest.Protocol); };
                        o.EnrichWithHttpResponse = (activity, httpResponse) => { activity.SetTag("responseLength", httpResponse.ContentLength); };

                        // automatically sets Activity Status to Error if an unhandled exception is thrown
                        o.RecordException = openTelemetryParameters.RecordException;
                        o.EnrichWithException = (activity, exception) =>
                        {
                            activity.SetTag("exceptionType", exception.GetType().ToString());
                            activity.SetTag("stackTrace", exception.StackTrace);
                        };
                    });
                    builder.AddOtlpExporter(otlp =>
                    {
                        otlp.Endpoint = new Uri("http://localhost:4317");
                    });
                });
            //.WithMetrics(builder =>
            //{
            //    builder.AddAspNetCoreInstrumentation()
            //    .AddHttpClientInstrumentation()
            //    .AddRuntimeInstrumentation()
            //    .AddProcessInstrumentation()
            //    .AddPrometheusExporter();
            //});


        }

        public static void AddOpenTelemetryMetrics(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OpenTelemetryParameters>(configuration.GetSection("OpenTelemetry"));
            var openTelemetryParameters = configuration.GetSection("OpenTelemetry").Get<OpenTelemetryParameters>();

            services.AddOpenTelemetry().WithMetrics(options =>
            {
                options.AddMeter(openTelemetryParameters.ServiceName);
                options.AddAspNetCoreInstrumentation();
                options.AddHttpClientInstrumentation();
                options.AddRuntimeInstrumentation();
                options.AddProcessInstrumentation();
                options.AddPrometheusExporter();
                options.ConfigureResource(resource =>
                {
                    resource.AddService(serviceName: openTelemetryParameters.ServiceName,
                        serviceVersion: openTelemetryParameters.ServiceVersion);
                });
            });
        }
    }
}
