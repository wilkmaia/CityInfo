using CityInfo.API.Exceptions;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CityInfo.API.Controllers;

[Authorize]
[ApiController]
[Route("api/cities/{cityId}/pointsofinterest")]
public class PointsOfInterestController : ControllerBase
{
    private readonly ILogger<PointsOfInterestController> _logger;
    private readonly IMailService _mailer;
    private readonly CityInfoRepository _cityInfoRepository;

    public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailer, CityInfoRepository cityInfoRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mailer = mailer ?? throw new ArgumentNullException(nameof(mailer));
        _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
    {
        try
        {
            var city = await _cityInfoRepository.GetCityById(cityId);
            return Ok(city.PointsOfInterest);
        }
        catch (INotFoundException)
        {
            _logger.LogInformation($"City with id {cityId} wasn't found");
            return NotFound();
        }
        catch (Exception e)
        {
            
            _logger.LogCritical(e, $"Exception while getting points of interest for city with id {cityId}.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{poiId}", Name = "GetPointOfInterestById")]
    public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterestById(int cityId, int poiId)
    {
        try
        {
            var poi = await _cityInfoRepository.GetPointOfInterestForCityById(cityId, poiId);
            return Ok(poi);
        }
        catch (INotFoundException)
        {
            return NotFound();
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, $"Exception while getting point of interest {poiId} for city with id {cityId}.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(
        int cityId,
        [FromBody] PointOfInterestForCreationDto poi
    )
    {
        try
        {
            var pointOfInterest = await _cityInfoRepository.CreatePointOfInterestForCity(cityId, poi);
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
        catch (INotFoundException)
        {
            return NotFound();
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, $"Exception while creating point of interest for city with id {cityId}.");
            return StatusCode(500, "Internal server error");
        }
    }
    
    [HttpPut("{poiId}")]
    public async Task<ActionResult> UpdatePointOfInterest(
        int cityId,
        int poiId,
        [FromBody] PointOfInterestForUpdateDto poi
    )
    {
        try
        {
            await _cityInfoRepository.UpdatePointOfInterest(cityId, poiId, poi);
            return NoContent();
        }
        catch (INotFoundException)
        {
            return NotFound();
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, $"Exception while updating point of interest {poiId} for city with id {cityId}.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPatch("{poiId}")]
    public async Task<ActionResult<PointOfInterestDto>> PatchPointOfInterest(
        int cityId,
        int poiId,
        [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument
    )
    {
        try
        {
            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityById(cityId, poiId);
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

            await _cityInfoRepository.UpdatePointOfInterest(cityId, poiId, poi);

            return NoContent();
        }
        catch (INotFoundException)
        {
            return NotFound();
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, $"Exception while patching point of interest {poiId} for city with id {cityId}.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{poiId}")]
    public async Task<ActionResult> DeletePointOfInterest(int cityId, int poiId)
    {
        try
        {
            await _cityInfoRepository.DeletePointOfInterest(cityId, poiId);
            
            _mailer.Send("Point of Interest Deleted", $"POI with id {poiId} for city with id {cityId} has been deleted.");
            
            return NoContent();
        }
        catch (INotFoundException)
        {
            return NotFound();
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, $"Exception while deleting point of interest {poiId} for city with id {cityId}.");
            return StatusCode(500, "Internal server error");
        }
    }
}