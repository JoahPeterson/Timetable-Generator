namespace Timetable.ExcelApi.Authentication;

/// <summary>
/// Class that represents the middleware that validates the Api-Key header.
/// </summary>
public class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IApiKeyValidation _apiKeyValidation;

    /// <summary>
    /// Instatiates a new instance of the <see cref="ApiKeyAuthMiddleware"/> class.
    /// </summary>
    /// <param name="next">Next item in the pipeline.</param>
    /// <param name="apiKeyValidation">Key Validation logic.</param>
    public ApiKeyAuthMiddleware(RequestDelegate next, IApiKeyValidation apiKeyValidation)
    {
        _next = next;
        _apiKeyValidation = apiKeyValidation;
    }

    /// <summary>
    /// Invoke method that validates the Api-Key header.
    /// </summary>
    /// <param name="context">The context for the api call.</param>
    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(AuthorizationContsants.ApiKeyHeaderName, out var extractedAppKey))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Incorrect request - Api-Key is missing from request header.");
            return;
        }

        var appKey = context.RequestServices.GetRequiredService<IConfiguration>().GetValue<string>("AppSettings:AppKey");

        if (!_apiKeyValidation.IsValidApiKey(extractedAppKey!))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized client - Api-Key is incorrect.");
            return;
        }

        await _next(context);
    }
}
