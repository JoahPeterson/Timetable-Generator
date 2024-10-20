using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimetableApp.DataModels.DataAccess;
public interface ICourseTypeData
{
    Task CreateAsync(CourseType courseType);
    Task<CourseType?> GetByIdAsync(string id);
    Task<List<CourseType>> GetAsync(bool IncludeArchived = false);
    Task<List<CourseType>> GetUsersCourseTypesAsync(string createdById);
    Task UpdateAsync(CourseType courseType);
}
