using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/cities/{cityId}/pointsofinterest")]
public class PointsOfInterestController : ControllerBase
{
    private readonly ILogger<PointsOfInterestController> _logger;

    public PointsOfInterestController(ILogger<PointsOfInterestController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
    {
        try
        {
            var city = CitiesDataStore.Current.Cities.Find(city => city.Id == cityId);
            if (city == null)
            {
                _logger.LogDebug($"City with id {cityId} wasn't found");
                return NotFound();
            }

            return Ok(city.PointsOfInterest);
        }
        catch (Exception e)
        {
            
            _logger.LogCritical(e, $"Exception while getting points of interest for city with id {cityId}.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{poiId}", Name = "GetPointOfInterestById")]
    public ActionResult<PointOfInterestDto> GetPointOfInterestById(int cityId, int poiId)
    {
        try
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
        catch (Exception e)
        {
            _logger.LogCritical(e, $"Exception while getting point of interest {poiId} for city with id {cityId}.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public ActionResult<PointOfInterestDto> CreatePointOfInterest(
        int cityId,
        [FromBody] PointOfInterestForCreationDto poi
    )
    {
        try
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
        catch (Exception e)
        {
            _logger.LogCritical(e, $"Exception while creating point of interest for city with id {cityId}.");
            return StatusCode(500, "Internal server error");
        }
    }
    
    [HttpPut("{poiId}")]
    public ActionResult UpdatePointOfInterest(
        int cityId,
        int poiId,
        [FromBody] PointOfInterestForUpdateDto poi
    )
    {
        try
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
        catch (Exception e)
        {
            _logger.LogCritical(e, $"Exception while updating point of interest {poiId} for city with id {cityId}.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPatch("{poiId}")]
    public ActionResult<PointOfInterestDto> PatchPointOfInterest(
        int cityId,
        int poiId,
        [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument
    )
    {
        try
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
        catch (Exception e)
        {
            _logger.LogCritical(e, $"Exception while patching point of interest {poiId} for city with id {cityId}.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{poiId}")]
    public ActionResult DeletePointOfInterest(int cityId, int poiId)
    {
        try
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

            city.PointsOfInterest.Remove(pointOfInterest);
            return NoContent();
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, $"Exception while deleting point of interest {poiId} for city with id {cityId}.");
            return StatusCode(500, "Internal server error");
        }
    }
}