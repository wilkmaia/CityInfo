namespace CityInfo.API.Exceptions;

public class CityNotFoundException : INotFoundException
{
    public CityNotFoundException(string keyName, string value) : base("City", keyName, value)
    {
    }
}