using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimetableApp.DataModels.DataAccess;
public interface IWorkUnitData
{
    Task CreateAsync(WorkUnit workUnit);
    Task<List<WorkUnit>> GetUsersAsync(string id);
    Task<WorkUnit?> GetByIdAsync(string id);
    Task<List<WorkUnit>> GetAsync();
    Task UpdateAsync(WorkUnit workUnit);
}
