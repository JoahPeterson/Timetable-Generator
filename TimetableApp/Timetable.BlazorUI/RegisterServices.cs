using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Timetable.BlazorUI.Components.Account;
using Timetable.BlazorUI.Data;
using Timetable.BlazorUI.Services;
using TimetableApp.DataModels.DataAccess;

namespace Timetable.BlazorUI;

/// <summary>
/// Provides extension methods for configuring services in the application.
/// </summary>
public static class RegisterServices
{
    /// <summary>
    /// Configures the services required by the application, including authentication, 
    /// authorization, database context, and other application-specific services.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> used to configure the application.</param>
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        builder.Services.AddDistributedMemoryCache(); 
        builder.Services.AddSession(options => 
        { options.IdleTimeout = TimeSpan.FromMinutes(15); options.Cookie.HttpOnly = true; options.Cookie.IsEssential = true; });

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddAuthorizationCore(config =>
        {
            config.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            config.AddPolicy("RegularUser", policy => policy.RequireAssertion(context => !context.User.IsInRole("Admin")));
        });

        builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        })
        .AddGoogle(googleOptions =>
        {
            googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
            googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        }).AddIdentityCookies();
        //.AddMicrosoftAccount(microsoftOptions =>
        //{
        //    microsoftOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
        //    microsoftOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
        //})



        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddMudServices();
        builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<CurrentUserService>();


        builder.Services.AddSingleton<IDbConnection, DbConnection>();
        builder.Services.AddSingleton<ICourseData, MongoCourseData>();
        builder.Services.AddSingleton<ICourseTypeData, MongoCourseTypeData>();
        builder.Services.AddSingleton<IHelpArticleData, MongoHelpArticleData>();
        builder.Services.AddSingleton<ILogData, MongoLogData>();
        builder.Services.AddSingleton<ITaskTypeData, MongoTaskTypeData>();
        builder.Services.AddSingleton<ITermData, MongoTermData>();
        builder.Services.AddSingleton<ITermDurationData, MongoTermDurationData>();
        builder.Services.AddSingleton<IUserData, MongoUserData>();
        builder.Services.AddSingleton<IWorkTaskData, MongoWorkTaskData>();
        builder.Services.AddSingleton<IWorkUnitTaskData, MongoWorkUnitTaskData>();
        builder.Services.AddSingleton<IWorkUnitData, MongoWorkUnitData>();
        
        builder.Services.AddSingleton<WorkUnitDateService>();
        builder.Services.AddSingleton<DuplicationService>();


        builder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddProvider(new TimetableLoggerProvider(
                builder.Services.BuildServiceProvider().GetRequiredService<ILogData>(),
                builder.Services.BuildServiceProvider().GetRequiredService<IUserData>(),
                builder.Services.BuildServiceProvider().GetRequiredService<CurrentUserService>()
            ));
        });


        builder.Services.AddScoped(sp => {
            var client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44316")
            };

            // Add the API key as a custom header
            client.DefaultRequestHeaders.Add("x-api-key", builder.Configuration["ExcelWebApiKey"]);

            return client;
        });

    }
}
