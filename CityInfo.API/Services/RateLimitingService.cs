using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;

namespace CityInfo.API.Services;

/// <inheritdoc />
public class RateLimitingService : IpRateLimitMiddleware
{
    /// <inheritdoc />
    public RateLimitingService(RequestDelegate next,
        IProcessingStrategy processingStrategy,
        IOptions<IpRateLimitOptions> options,
        IIpPolicyStore policyStore,
        IRateLimitConfiguration config,
        ILogger<IpRateLimitMiddleware> logger) : base(next,
        processingStrategy,
        options,
        policyStore,
        config,
        logger)
    {
    }

    /// <inheritdoc />
    public override Task ReturnQuotaExceededResponse(HttpContext httpContext, RateLimitRule rule, string retryAfter)
    {
        if (httpContext == null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }
        
        httpContext.Response.Headers["Retry-After"] = retryAfter;
        httpContext.Response.StatusCode = 429;
        httpContext.Response.ContentType = "application/json";

        var problemDetails = httpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>()
            .CreateProblemDetails(httpContext,
                429,
                "Too many requests",
                detail: "Rate Limit Exceeded",
                type: "https://www.rfc-editor.org/rfc/rfc6585#section-4");

        return httpContext.Response.WriteAsJsonAsync(problemDetails);
    }
}