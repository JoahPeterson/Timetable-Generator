using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimetableApp.DataModels.DataAccess
{
    public interface ICourseData
    {
        Task CreateCourseAsync(Course course);
        Task<List<Course>> GetUsersCoursesAsync(string id);
        Task<Course?> GetCourseAsync(string id);
        Task<List<Course>> GetCoursesAsync();
        Task UpdateCourseAsync(Course course);
    }
}
