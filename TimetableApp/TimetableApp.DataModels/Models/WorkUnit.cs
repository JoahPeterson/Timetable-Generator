namespace TimetableApp.DataModels.Models;

public class WorkUnit
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public Auditable AuditInformation { get; set; } = new();

    public string CourseId { get; set; }

    public int Duration { get; set; }

    public string Name { get; set; }

    public int SequenceNumber { get; set; }

    public List<WorkUnitTask>? Tasks { get; set; } = new();

}
