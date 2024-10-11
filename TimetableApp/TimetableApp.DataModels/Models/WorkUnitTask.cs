namespace TimetableApp.DataModels.Models;

public class WorkUnitTask
{
    public string Id { get; set; }

    public Auditable AuditInformation { get; set; }

    public string? Duration { get; set; }

    public DateTime? DueDate { get; set; }

    public string TaskId { get; set; }

    public int? SequenceNumber { get; set; }

    public string WorkUnitId { get; set; }

}
