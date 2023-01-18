using System.Reflection;
using System.Text;
using AspNetCoreRateLimit;
using CityInfo.API.Data;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Formatting.Compact;
using ServiceStack;
using ServiceStack.Caching;
using ServiceStack.Redis;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

// Unnecessary -- simply illustrating how logging providers could be manually handled
// builder.Logging.ClearProviders();
// builder.Logging.AddConsole();

builder.Host.UseSerilog((builderContext, services, loggerConfiguration) => loggerConfiguration
    .Enrich.With<TraceIdInjector>()
    .Enrich.WithProperty("Application", builderContext.HostingEnvironment.ApplicationName)
    .Enrich.WithProperty("Environment", builderContext.HostingEnvironment.EnvironmentName)
    .Enrich.WithProperty("Hostname", System.Net.Dns.GetHostName())
    .Enrich.FromLogContext()
    .WriteTo.Console(new RenderedCompactJsonFormatter())
    .WriteTo.File("logs/cityinfo.txt", rollingInterval: RollingInterval.Day));

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
})
    .AddNewtonsoftJson()
    .AddXmlSerializerFormatters()
    .AddXmlDataContractSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction =>
{
    string xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
    
    setupAction.IncludeXmlComments(xmlCommentsFullPath);
    
    setupAction.AddSecurityDefinition("CityInfoApiBearerAuth", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "Input a valid token to access this API",
    });
    
    setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "CityInfoApiBearerAuth",
                },
            },
            new List<string>()
        }
    });
});

builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
builder.Services.AddSqlite<CityInfoContext>("Data Source=CityInfo.db");
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"])),
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBeFromTeresina", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("city", "Teresina");
    });
});

builder.Services.AddApiVersioning(setupAction =>
{
    setupAction.AssumeDefaultVersionWhenUnspecified = true;
    setupAction.DefaultApiVersion = new ApiVersion(1, 0);
    setupAction.ReportApiVersions = true;
});

builder.Services.AddMemoryCache();
builder.Services.AddInMemoryRateLimiting();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
// If there are specific policies for sets of ips
// See https://github.com/stefanprodan/AspNetCoreRateLimit/wiki/IpRateLimitMiddleware#setup for more information
// services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var appHost = new GenericAppHost().Init();
// appHost.Register<ICacheClient>(new MemoryCacheClient());
appHost.Register<IRedisClientsManagerAsync>(new RedisManagerPool("SfJ0rDebntftbhNCsoDgLHTu4al1mhKJ@redis-12344.c56.east-us.azure.cloud.redislabs.com:12344"));
appHost.Register<ICacheClientAsync>(await appHost.Resolve<IRedisClientsManagerAsync>().GetCacheClientAsync());
builder.Services.AddScoped<CacheService>();

#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();
#else
builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

builder.Services.AddScoped<CityInfoRepository>();

var app = builder.Build();

app.UseMiddleware<RateLimitingService>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Ensure database exists and seed it if needed
    app.CreateDbIfNotExists();
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseResponseCompression();

app.MapControllers();

app.Run();