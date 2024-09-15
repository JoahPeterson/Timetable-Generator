﻿using Microsoft.AspNetCore.Identity;
using Timetable.BlazorUI.Data;

namespace Timetable.BlazorUI.SetupServices;

public class SetupFunctions
{
    internal static async Task CreateAdminUser(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string adminRole = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        string adminEmail = "admin@example.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var defaultAdmin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            var result = await userManager.CreateAsync(defaultAdmin, "Admin@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(defaultAdmin, adminRole);
            }
        }
    }

    internal static async Task CreateCourseTypeData(ICourseTypeData courseTypeData)
    {
        var courseTypes = await courseTypeData.GetAsync();
        if (courseTypes.Any())
            return;

        var courseType = new CourseType();
        courseType.Name = "Online";
        courseType.ToolTip = "This course is 100% online";
        await courseTypeData.CreateAsync(courseType);

        courseType = new CourseType();
        courseType.Name = "Hydrid";
        courseType.ToolTip = "This course is a mix of online and in person";
        await courseTypeData.CreateAsync(courseType);

        courseType = new CourseType();
        courseType.Name = "In Person";
        courseType.ToolTip = "This course is 100% in person";
        await courseTypeData.CreateAsync(courseType);
    }
}
