using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Timetable.BlazorUI.Components.Account;
using Timetable.BlazorUI.Data;
using TimetableApp.DataModels.DataAccess;
using Timetable.BlazorUI.SetupServices;

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

        builder.Services.AddAuthorizationCore(config =>
        {
            config.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
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
        })
        .AddMicrosoftAccount(microsoftOptions =>
        {
            microsoftOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
            microsoftOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
        })
        .AddIdentityCookies();
        

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        builder.Services.AddBlazoredModal();
        builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
        builder.Services.AddScoped<UserService>();


        builder.Services.AddSingleton<IDbConnection, DbConnection>();
        builder.Services.AddSingleton<ITaskTypeData, MongoTaskTypeData>();
        builder.Services.AddSingleton<IUserData, MongoUserData>();
        builder.Services.AddSingleton<ICourseTypeData, MongoCourseTypeData>();
        builder.Services.AddSingleton<ICourseData, MongoCourseData>();
        builder.Services.AddSingleton<ITermData, MongoTermData>();
        builder.Services.AddSingleton<ITermDurationData, MongoTermDurationData>();

    }
}
