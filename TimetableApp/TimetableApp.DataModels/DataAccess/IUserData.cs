using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimetableApp.DataModels.DataAccess;
public interface IUserData
{
    Task CreateAsync(User user);
    Task<User?> GetByIdAsync(string id);
    Task<List<User>> GetUsersAsync();

    Task<User> GetUserFromAuthenticationAsync(string objectId);
    Task UpdateAsync(User user);
}
