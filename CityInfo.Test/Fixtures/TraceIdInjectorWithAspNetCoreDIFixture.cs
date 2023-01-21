using CityInfo.API.Services;
using CityInfo.Test.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;

namespace CityInfo.Test.Fixtures;

public class TraceIdInjectorWithAspNetCoreDIFixture : IDisposable
{
    private readonly ServiceProvider _serviceProvider;

    public ILogEventPropertyFactory TestLogEventPropertyFactory =>
        _serviceProvider.GetService<ILogEventPropertyFactory>();

    public ILogEventEnricher TestTraceIdInjector => _serviceProvider.GetService<ILogEventEnricher>();

    public TraceIdInjectorWithAspNetCoreDIFixture()
    {
        var services = new ServiceCollection();
        services.AddScoped<ILogEventPropertyFactory, TestLogEventPropertyFactory>();
        services.AddScoped<ILogEventEnricher, TraceIdInjector>();
        _serviceProvider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        _serviceProvider.Dispose();
    }
}