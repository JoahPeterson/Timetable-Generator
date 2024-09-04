namespace Timetable.ExcelApi.Authentication;

/// <summary>
/// Class which handles api key validation functionality.
/// </summary>
public class ApiKeyValidation : IApiKeyValidation
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Instantiates a new instace of the APIKeyValidation.
    /// </summary>
    /// <param name="configuration"></param>
    public ApiKeyValidation(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool IsValidApiKey(string clientApiKey)
    {
        if (string.IsNullOrEmpty(clientApiKey))
            return false;

        var apiKey = _configuration.GetValue<string>(AuthorizationContsants.AppSecretHeaderName);

        if (apiKey is null || apiKey != clientApiKey)
            return false;

        return true;
    }
}
