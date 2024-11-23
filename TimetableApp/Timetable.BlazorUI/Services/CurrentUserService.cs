namespace Timetable.BlazorUI.Services
{
    using Microsoft.AspNetCore.Components.Authorization;
    using System.Security.Claims;

    public class CurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(c => c.Type.Contains("nameidentifier"))?.Value;
        }

        public ClaimsPrincipal? GetUser()
        {
            return _httpContextAccessor.HttpContext?.User;
        }
    }
}
