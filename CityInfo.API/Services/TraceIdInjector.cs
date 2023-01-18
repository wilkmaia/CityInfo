using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace CityInfo.API.Services;

/// <summary>
/// Middleware to inject the request's trace id to the logger instance
/// </summary>
public class TraceIdInjector : ILogEventEnricher
{
    /// <inheritdoc />
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        string? traceId = Activity.Current?.Id;
        if (traceId != null)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", Activity.Current?.Id));
        }
    }
}