﻿namespace TimetableApp.DataModels.Models;

public class User
{
    public string Id { get; set; }

    public List<Course>? Courses { get; set; }

    public List<CourseType>? CoursesType { get; set; }

    public string DisplayName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public List<WorkTask>? Tasks { get; set; }

    public List<TaskType>? TaskTypes { get; set; }

}
