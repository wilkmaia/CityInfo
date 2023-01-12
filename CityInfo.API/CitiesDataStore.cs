using CityInfo.API.Models;

namespace CityInfo.API;

public class CitiesDataStore
{
    public List<CityDto> Cities { get; set; }
    public static CitiesDataStore Current { get; } = new CitiesDataStore();
    
    public CitiesDataStore()
    {
        Cities = new List<CityDto>()
        {
            new CityDto()
            {
                Id = 1,
                Name = "Teresina",
                Description = "Capital do Piauí",
            },
            new CityDto()
            {
                Id = 2,
                Name = "Timon",
                Description = "Quintal de Teresina",
            },
            new CityDto()
            {
                Id = 3,
                Name = "São João dos Patos",
            },
        };
    }
}