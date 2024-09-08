using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimetableApp.DataModels.DataAccess;
public interface ITaskTypeData
{
    Task ArchiveTaskTypeAsync(string id);
    Task CreateTaskTypeAsync(TaskType type);
    Task<TaskType?> GetTaskTypeByIdAsync(string id);
    Task<List<TaskType>> GetTaskTypesAsync();
    Task UpdateTaskTypeAsync(TaskType type);
}
