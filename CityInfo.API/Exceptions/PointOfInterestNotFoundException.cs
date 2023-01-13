namespace CityInfo.API.Exceptions;

public class PointOfInterestNotFoundException : INotFoundException
{
    public PointOfInterestNotFoundException(string keyName, string value)
        : base("Point of Interest", keyName, value)
    {
    }
}