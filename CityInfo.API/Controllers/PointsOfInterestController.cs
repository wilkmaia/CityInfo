using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/cities/{cityId}/pointsofinterest")]
public class PointsOfInterestController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
    {
        var city = CitiesDataStore.Current.Cities.Find(city => city.Id == cityId);
        if (city == null)
        {
            return NotFound();
        }

        return Ok(city.PointsOfInterest);
    }

    [HttpGet("{poiId}")]
    public ActionResult<PointOfInterestDto> GetPointOfInterestById(int cityId, int poiId)
    {
        var city = CitiesDataStore.Current.Cities.Find(city => city.Id == cityId);
        if (city == null)
        {
            return NotFound();
        }

        var poi = city.PointsOfInterest.FirstOrDefault(poi => poi.Id == poiId);
        if (poi == null)
        {
            return NotFound();
        }

        return Ok(poi);
    }
}