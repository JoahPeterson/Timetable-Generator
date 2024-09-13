namespace TimetableApp.DataModels.Models;

public class TaskType
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public Auditable AuditInformation { get; set; }

    public string CreatedById { get; set; }

    public string Description { get; set; }

    public string Name { get; set; }

}
