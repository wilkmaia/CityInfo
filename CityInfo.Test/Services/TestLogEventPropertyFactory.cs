using Serilog.Core;
using Serilog.Events;

namespace CityInfo.Test.Services;

public class TestLogEventPropertyFactory : ILogEventPropertyFactory
{
    public LogEventProperty CreateProperty(string name, object value, bool destructureObjects = false) =>
        new(name, new ScalarValue(value));
}
