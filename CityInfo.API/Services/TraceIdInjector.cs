using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace CityInfo.API.Services;

/// <summary>
/// Middleware to inject the request's trace id to the logger instance
/// </summary>
public class TraceIdInjector : ILogEventEnricher
{
    private static string GetTraceId() => Activity.Current?.Id ?? "test-trace-id";

    /// <inheritdoc />
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var traceId = GetTraceId();
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", traceId));
    }
}