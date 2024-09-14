using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimetableApp.DataModels.DataAccess;
public interface ITaskTypeData
{
    Task CreateTaskTypeAsync(TaskType type);
    Task<List<TaskType>> GetUsersTaskTypesAsync(string id);
    Task<TaskType?> GetTaskTypeAsync(string id);
    Task<List<TaskType>> GetTaskTypesAsync();
    Task UpdateTaskTypeAsync(TaskType type);
}
