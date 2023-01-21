using CityInfo.Test.Fixtures;
using Serilog.Events;
using Serilog.Parsing;

namespace CityInfo.Test;

public class TraceIdInjectorTests : IClassFixture<TraceIdInjectorWithAspNetCoreDIFixture>
{
    private readonly TraceIdInjectorWithAspNetCoreDIFixture _traceIdInjectorFixture;

    public TraceIdInjectorTests(TraceIdInjectorWithAspNetCoreDIFixture traceIdInjectorFixture)
    {
        _traceIdInjectorFixture = traceIdInjectorFixture;
    }

    [Fact]
    public void Enrich_InjectLogEventProperty_TraceIdShouldBeValid()
    {
        var traceIdInjector = _traceIdInjectorFixture.TestTraceIdInjector;
        var logEventPropertyFactory = _traceIdInjectorFixture.TestLogEventPropertyFactory;

        var logEvent = new LogEvent(new DateTimeOffset(),
            LogEventLevel.Information,
            null,
            new MessageTemplate(new MessageTemplateToken[]
            {
            }),
            new LogEventProperty[]
            {
            });
        
        Assert.False(logEvent.Properties.ContainsKey("TraceId"));
        traceIdInjector.Enrich(logEvent, logEventPropertyFactory);
        Assert.True(logEvent.Properties.ContainsKey("TraceId"));

        logEvent.Properties.TryGetValue("TraceId", out LogEventPropertyValue traceId);
        Assert.NotNull(traceId);
    }
}