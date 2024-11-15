using Microsoft.OpenApi.Models;
using Timetable.ExcelApi.Authentication;
using Timetable.ExcelApi.Services;
using TimetableApp.DataModels.DataAccess;

namespace Timetable.ExcelApi;

public static class RegisterServices
{
    internal static void ConfigureServices(this WebApplicationBuilder builder) 
    {
        builder.Services.AddControllers();

        builder.Services.AddTransient<IApiKeyValidation, ApiKeyValidation>();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Description = "API Key needed to access the endpoints. You can get it from the /api-key endpoint)",
                Type = SecuritySchemeType.ApiKey,
                Name = "x-api-key",
                In = ParameterLocation.Header,
                Scheme = "ApiKeyScheme"

            });
            var scheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                In = ParameterLocation.Header
            };
            var requirement = new OpenApiSecurityRequirement
            {
                { scheme, new string[] { } }
            };
            c.AddSecurityRequirement(requirement);
        });

        builder.Services.AddSingleton<IDbConnection, DbConnection>();
        builder.Services.AddSingleton<ICourseData, MongoCourseData>();
        builder.Services.AddSingleton<ICourseTypeData, MongoCourseTypeData>();
        builder.Services.AddSingleton<IHelpArticleData, MongoHelpArticleData>();
        builder.Services.AddSingleton<ITaskTypeData, MongoTaskTypeData>();
        builder.Services.AddSingleton<ITermData, MongoTermData>();
        builder.Services.AddSingleton<ITermDurationData, MongoTermDurationData>();
        builder.Services.AddSingleton<IUserData, MongoUserData>();
        builder.Services.AddSingleton<IWorkTaskData, MongoWorkTaskData>();
        builder.Services.AddSingleton<IWorkUnitTaskData, MongoWorkUnitTaskData>();
        builder.Services.AddSingleton<IWorkUnitData, MongoWorkUnitData>();

        builder.Services.AddTransient<ExcelDocumentManager>();
        builder.Configuration.AddUserSecrets<Program>();
    }
}
