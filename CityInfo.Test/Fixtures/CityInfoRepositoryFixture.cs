using AutoMapper;
using CityInfo.API.Data;
using CityInfo.API.Entities;
using CityInfo.API.Profiles;
using CityInfo.API.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;
using ServiceStack.Caching;

namespace CityInfo.Test.Fixtures;

public class CityInfoRepositoryFixture : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    public readonly CityInfoContext TestDbContext;
    private IMapper TestMapper => _serviceProvider.GetService<IMapper>();
    private CacheService TestCacheService => _serviceProvider.GetService<CacheService>();

    public CityInfoRepository TestCityInfoRepository;
    
    public CityInfoRepositoryFixture()
    {
        var services = new ServiceCollection();
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        var optionsBuilder = new DbContextOptionsBuilder<CityInfoContext>()
            .UseSqlite(connection);
        TestDbContext = new CityInfoContext(optionsBuilder.Options);
        TestDbContext.Database.Migrate();
        SeedData.Initialize(TestDbContext);
        
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        
        var appHost = new GenericAppHost().Init();
        appHost.Register<ICacheClient>(new MemoryCacheClient());
        services.AddScoped<CacheService>();
        
        _serviceProvider = services.BuildServiceProvider();

        TestCityInfoRepository = new CityInfoRepository(TestDbContext, TestMapper, TestCacheService);
    }

    public void Dispose()
    {
        _serviceProvider.Dispose();
    }
}