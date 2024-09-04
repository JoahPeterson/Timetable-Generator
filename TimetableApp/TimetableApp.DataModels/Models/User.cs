namespace Timetable.BlazorUI.Data.Models
{
    public class User
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string DisplayName { get; set; }

        public List<Course>? Courses { get; set; }

        public List<CourseType>? CoursesType { get; set; }

        public List<Task>? Tasks { get; set; }

        public List<TaskType>? TaskTypes { get; set; }
    }
}
