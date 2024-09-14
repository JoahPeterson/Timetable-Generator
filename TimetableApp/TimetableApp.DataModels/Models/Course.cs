using System.ComponentModel.DataAnnotations;

namespace TimetableApp.DataModels.Models;

public class Course
{
    public string Id { get; set; }

    public Auditable AuditInformation { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 100 characters.")]
    public string? Description { get; set; }

    [DataType(DataType.Date, ErrorMessage ="Value must be a valid date.")]
    public DateTime EndDate { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 30 characters.")]
    public string Name { get; set; }

    [DataType(DataType.Date, ErrorMessage = "Value must be a valid date.")]
    public DateTime StartDate { get; set; }

    public Term Term { get; set; }

    public List<WorkUnit> WorkUnits { get; set; }

}
