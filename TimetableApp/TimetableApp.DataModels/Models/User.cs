namespace TimetableApp.DataModels.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public Auditable AuditInformation { get; set; } = new();

    public List<Course>? Courses { get; set; } = new();

    public List<CourseType>? CoursesType { get; set; } = new();

    public string DisplayName { get; set; }

    public string EmailAddress { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string ObjectIdentifier { get; set; }

    public List<WorkTask>? Tasks { get; set; } = new();

    public List<TaskType>? TaskTypes { get; set; } = new();

}
