namespace Timetable.BlazorUI.Data.Models
{
    public class User
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string DisplayName { get; set; }

        public IEnumerable<Course>? Courses { get; set; }

        public IEnumerable<CourseType>? CoursesType { get; set; }

        public IEnumerable<Task>? Tasks { get; set; }

        public IEnumerable<TaskType>? TaskTypes { get; set; }
    }
}
