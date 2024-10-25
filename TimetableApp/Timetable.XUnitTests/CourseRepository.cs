using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timetable.XUnitTests
{
    public class CourseRepository : ICourseRepository
    {
        private readonly Dictionary<string, Course> _courses = new();

        public void AddCourse(Course course)
        {
            _courses[course.Id] = course;
        }

        public async Task<Course?> GetCourseAsync(string id)
        {
            _courses.TryGetValue(id, out var course);
            return await Task.FromResult(course);
        }
    }
}
