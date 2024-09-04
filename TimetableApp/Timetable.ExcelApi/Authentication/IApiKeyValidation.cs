namespace Timetable.ExcelApi.Authentication;

public interface IApiKeyValidation
{
    bool IsValidApiKey(string clientApiKey);
}
