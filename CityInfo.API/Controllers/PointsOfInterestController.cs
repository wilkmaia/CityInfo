using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
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

    [HttpGet("{poiId}", Name = "GetPointOfInterestById")]
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

    [HttpPost]
    public ActionResult<PointOfInterestDto> CreatePointOfInterest(
        int cityId,
        [FromBody] PointOfInterestForCreationDto poi
    )
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == cityId);
        if (city == null)
        {
            return NotFound();
        }

        // TODO - update approach when database is added to project
        int nextPointOfInterestId = CitiesDataStore.Current.Cities
            .SelectMany(c => c.PointsOfInterest)
            .Max(p => p.Id) + 1;

        var pointOfInterest = new PointOfInterestDto()
        {
            Id = nextPointOfInterestId,
            Name = poi.Name,
            Description = poi.Description,
        };
        
        city.PointsOfInterest.Add(pointOfInterest);

        return CreatedAtRoute(
            "GetPointOfInterestById", 
            new
            {
                cityId,
                poiId = pointOfInterest.Id,
            },
            pointOfInterest
        );
    }
    
    [HttpPut("{poiId}")]
    public ActionResult UpdatePointOfInterest(
        int cityId,
        int poiId,
        [FromBody] PointOfInterestForUpdateDto poi
    )
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == cityId);
        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == poiId);
        if (pointOfInterest == null)
        {
            return NotFound();
        }

        pointOfInterest.Name = poi.Name;
        pointOfInterest.Description = poi.Description;

        return NoContent();
    }

    [HttpPatch("{poiId}")]
    public ActionResult<PointOfInterestDto> PatchPointOfInterest(
        int cityId,
        int poiId,
        [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument
    )
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == cityId);
        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == poiId);
        if (pointOfInterest == null)
        {
            return NotFound();
        }

        var poi = new PointOfInterestForUpdateDto()
        {
            Name = pointOfInterest.Name,
            Description = pointOfInterest.Description,
        };
        
        patchDocument.ApplyTo(poi, ModelState);
        if (!ModelState.IsValid || !TryValidateModel(poi))
        {
            return BadRequest(ModelState);
        }

        pointOfInterest.Name = poi.Name;
        pointOfInterest.Description = poi.Description;

        return NoContent();
    }
}