using System.ComponentModel.DataAnnotations;

namespace TimetableApp.DataModels.Models;

public class WorkUnit
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public Auditable AuditInformation { get; set; } = new();

    public string? CourseId { get; set; }

    [Required(ErrorMessage = "Duration is required")]
    public int Duration { get; set; } = 1;

    [Required(ErrorMessage = "Name is required")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 30 characters.")]
    public string? Name { get; set; }

    public int SequenceNumber { get; set; }

    public List<WorkUnitTask>? Tasks { get; set; } = new();


}
