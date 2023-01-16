using CityInfo.API.Exceptions;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/cities")]
public class CitiesController : ControllerBase
{
    private readonly CityInfoRepository _cityInfoRepository;
    
    public CitiesController(CityInfoRepository cityInfoRepository)
    {
        _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerator<CityDto>>> GetCities([FromQuery(Name = "nameFilter")] string? name)
    {
        var cities = await _cityInfoRepository.GetCities(name);
        return Ok(cities);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CityDto>> GetCity(int id)
    {
        try
        {
            CityDto city = await _cityInfoRepository.GetCityById(id);
            return Ok(city);
        }
        catch (CityNotFoundException)
        {
            return NotFound();
        }
    }
}