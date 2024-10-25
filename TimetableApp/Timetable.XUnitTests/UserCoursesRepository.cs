using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timetable.XUnitTests
{
    public class UserCoursesRepository : IUserCoursesRepository
    {
        private readonly List<Course> _courses = new();

        public void AddCourse(Course course)
        {
            _courses.Add(course);
        }

        public async Task<List<Course>> GetUsersCoursesAsync(string createdById)
        {
            try
            {
                return await Task.FromResult(_courses
                    .Where(course =>
                        course.AuditInformation.CreatedById == createdById &&
                        !course.AuditInformation.IsArchived)
                    .ToList());
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
