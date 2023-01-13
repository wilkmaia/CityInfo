using CityInfo.API.Exceptions;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/cities")]
public class CitiesController : ControllerBase
{
    private readonly CityInfoService _cityInfoService;
    
    public CitiesController(CityInfoService cityInfoService)
    {
        _cityInfoService = cityInfoService ?? throw new ArgumentNullException(nameof(cityInfoService));
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerator<CityDto>>> GetCities()
    {
        return Ok(await _cityInfoService.GetAllCities());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CityDto>> GetCity(int id)
    {
        try
        {
            CityDto city = await _cityInfoService.GetCityById(id);
            return Ok(city);
        }
        catch (CityNotFoundException)
        {
            return NotFound();
        }
    }
}