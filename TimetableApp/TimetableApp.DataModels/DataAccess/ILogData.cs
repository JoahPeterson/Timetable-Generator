using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimetableApp.DataModels.DataAccess
{
    public interface ILogData
    {
        Task CreateAsync(Log log);
        Task<List<Log>> GetByUserIdAsync(string loggedInUserId);
        Task<List<Log>> GetLogsAsync();
    }
}
