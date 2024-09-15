namespace TimetableApp.DataModels.Models;

public class CourseType
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public Auditable AuditInformation { get; set; } = new ();

    public string? ToolTip { get; set; }

    public string? Name { get; set; }
}
