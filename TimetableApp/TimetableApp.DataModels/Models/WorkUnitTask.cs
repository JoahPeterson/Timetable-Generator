using System.ComponentModel.DataAnnotations;

namespace TimetableApp.DataModels.Models;

public class WorkUnitTask
{
    public string Id { get; set; }

    public Auditable AuditInformation { get; set; }

    public DateTime? DueDate { get; set; }

    [RegularExpression("^[0-9]*$", ErrorMessage = "Only numeric values are allowed.")]
    public string? Duration { get; set; }

    public string TaskId { get; set; }

    public int? SequenceNumber { get; set; }

    public string WorkUnitId { get; set; }

}
