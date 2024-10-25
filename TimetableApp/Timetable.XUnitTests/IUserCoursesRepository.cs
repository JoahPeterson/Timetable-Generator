using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timetable.XUnitTests
{
    public interface IUserCoursesRepository
    {
        Task<List<Course>> GetUsersCoursesAsync(string createdById);
    }
}
