namespace CityInfo.API.Exceptions;

public abstract class INotFoundException : Exception
{
    protected INotFoundException(string entityName, string keyName, string value)
        : base($"{entityName} with {keyName} {value} not found in database")
    {
    }
}