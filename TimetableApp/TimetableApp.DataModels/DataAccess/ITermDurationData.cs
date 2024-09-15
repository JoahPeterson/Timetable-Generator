using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimetableApp.DataModels.DataAccess;
public interface ITermDurationData
{
    Task CreateAsync(TermDuration termDuration);
    Task<TermDuration?> GetByIdAsync(string id);
    Task<List<TermDuration>> GetAsync();
    Task UpdateAsync(TermDuration termDuration);
}
