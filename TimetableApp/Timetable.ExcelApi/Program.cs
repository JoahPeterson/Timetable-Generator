
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Timetable.ExcelApi.Authentication;

namespace Timetable.ExcelApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.ConfigureServices();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseMiddleware<ApiKeyAuthMiddleware>();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
