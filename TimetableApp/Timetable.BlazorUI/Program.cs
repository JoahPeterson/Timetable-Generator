using Timetable.BlazorUI;
using Timetable.BlazorUI.Components;
using Timetable.BlazorUI.SetupServices;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureServices();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.UseSession();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// Creates an admin user if they do not exist already...
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SetupFunctions.CreateAdminUser(services).Wait();
}

//SetupFunctions.CreateCourseTypeData(app.Services.GetRequiredService<ICourseTypeData>()).Wait();

//SetupFunctions.CreateTermData(app.Services.GetRequiredService<ITermData>(), app.Services.GetRequiredService<ITermDurationData>()).Wait();
app.Run();

