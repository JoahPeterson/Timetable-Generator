using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimetableApp.DataModels.DataAccess
{
    public interface IWorkTaskData
    {
        Task CreateAsync(WorkTask task);
        Task<WorkTask?> GetByIdAsync(string id);
        Task<List<WorkTask>> GetAsync();
        Task UpdateAsync(WorkTask task);
    }
}
