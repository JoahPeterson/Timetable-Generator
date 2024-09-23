using System.ComponentModel.DataAnnotations;

namespace TimetableApp.DataModels.Models;

public class Term
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public Auditable AuditInformation { get; set; } = new();

    public int Duration { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 30 characters.")]
    public string? Name { get; set; }

    public string ToolTip { get; } = "Time of year selection.";
}
