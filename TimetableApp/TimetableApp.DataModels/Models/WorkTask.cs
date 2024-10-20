using System.ComponentModel.DataAnnotations;

namespace TimetableApp.DataModels.Models;

public class WorkTask
{

    public string Id { get; set; }

    public Auditable AuditInformation { get; set; } = new Auditable();

    [Required(ErrorMessage = "Description is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 100 characters.")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 30 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Task type is required")]
    public string TypeId { get; set; }
}
