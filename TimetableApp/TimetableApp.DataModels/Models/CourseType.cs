using System.ComponentModel.DataAnnotations;

namespace TimetableApp.DataModels.Models;

public class CourseType
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public Auditable AuditInformation { get; set; } = new ();

    [Required(ErrorMessage = "Description is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 100 characters.")]
    public string? ToolTip { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 30 characters.")]
    public string? Name { get; set; }
}
