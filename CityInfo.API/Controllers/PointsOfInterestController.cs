using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/cities/{cityId}/pointsofinterest")]
public class PointsOfInterestController : ControllerBase
{
    private readonly ILogger<PointsOfInterestController> _logger;
    private readonly IMailService _mailer;
    private readonly CitiesDataStore _citiesDataStore;

    public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailer, CitiesDataStore citiesDataStore)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mailer = mailer ?? throw new ArgumentNullException(nameof(mailer));
        _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
    {
        try
        {
            var city = _citiesDataStore.Cities.Find(city => city.Id == cityId);
            if (city == null)
            {
                _logger.LogInformation($"City with id {cityId} wasn't found");
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
            var city = _citiesDataStore.Cities.Find(city => city.Id == cityId);
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
            var city = _citiesDataStore.Cities.FirstOrDefault(city => city.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            // TODO - update approach when database is added to project
            int nextPointOfInterestId = _citiesDataStore.Cities
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
            var city = _citiesDataStore.Cities.FirstOrDefault(city => city.Id == cityId);
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
            var city = _citiesDataStore.Cities.FirstOrDefault(city => city.Id == cityId);
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
            var city = _citiesDataStore.Cities.FirstOrDefault(city => city.Id == cityId);
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
            _mailer.Send("Point of Interest Deleted", $"POI with id {poiId} for city with id {cityId} has been deleted.");
            
            return NoContent();
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, $"Exception while deleting point of interest {poiId} for city with id {cityId}.");
            return StatusCode(500, "Internal server error");
        }
    }
}