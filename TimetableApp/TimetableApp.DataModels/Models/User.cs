namespace TimetableApp.DataModels.Models;

public class User
{
    public User()
    {
        AuditInformation = new Auditable();
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public Auditable AuditInformation { get; set; }

    public List<Course>? Courses { get; set; }

    public List<CourseType>? CoursesType { get; set; }

    public string DisplayName { get; set; }

    public string EmailAddress { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string ObjectIdentifier { get; set; }

    public List<WorkTask>? Tasks { get; set; }

    public List<TaskType>? TaskTypes { get; set; }

}
