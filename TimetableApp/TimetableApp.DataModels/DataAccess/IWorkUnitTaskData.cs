using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimetableApp.DataModels.DataAccess
{
    public interface IWorkUnitTaskData
    {
        Task CreateAsync(WorkUnitTask task);
        Task<WorkUnitTask?> GetByIdAsync(string id);
        Task<List<WorkUnitTask>> GetAsync();
        Task<List<WorkUnitTask>> GetUsersWorkUnitTasksAsync(string id);
        Task UpdateAsync(WorkUnitTask task);
    }
}
