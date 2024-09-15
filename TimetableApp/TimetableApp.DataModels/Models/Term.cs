namespace TimetableApp.DataModels.Models;

public class Term
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public Auditable AuditInformation { get; set; } = new();

    public int Duration { get; set; }

    public string? Name { get; set; }

    public string ToolTip { get; } = "Time of year selection.";
}
