namespace Timetable.ExcelApi.Authentication;

public class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IApiKeyValidation _apiKeyValidation;

    public ApiKeyAuthMiddleware(RequestDelegate next, IApiKeyValidation apiKeyValidation)
    {
        _next = next;
        _apiKeyValidation = apiKeyValidation;
    }

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
