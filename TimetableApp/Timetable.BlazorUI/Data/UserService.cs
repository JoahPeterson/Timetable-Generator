using Microsoft.AspNetCore.Identity;
using Timetable.BlazorUI.Data;

/// <summary>
/// Class which adds admin tool for managing the application users.
/// </summary>
public class UserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    /// <summary>
    /// Instantiates a new instance of the UserService class
    /// </summary>
    /// <param name="userManager">Instance of the Identity UserManagers.</param>
    /// <param name="roleManager">Instance of the Identity RolerManager.</param>
    public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Asynchronously assigns a specified role to a user identified by their email address.
    /// If the role does not exist, it will be created.
    /// </summary>
    /// <param name="email">The email address of the user to whom the role will be assigned.</param>
    /// <param name="role">The role to assign to the user.</param>
    public async Task AssignRole(string email, string role)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }

            // Remove all roles and add the new one
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, role);
        }
    }

    /// <summary>
    /// Asynchronously retrieves a list of all users who have registered with the application, 
    /// along with their assigned roles.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains a list of <see cref="UserModel"/> objects, 
    /// each containing the user's email and their assigned role.
    /// </returns>
    public async Task<List<UserModel>> GetAllUsersAsync()
    {
        var users = _userManager.Users.ToList();
        var userList = new List<UserModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userList.Add(new UserModel
            {
                Email = user.Email,
                Role = roles.FirstOrDefault() ?? "User" // Default to "User" if no roles are assigned
            });
        }

        return userList;
    }


}
