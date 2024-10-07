using System.ComponentModel.DataAnnotations;

namespace TimetableApp.DataModels.Models;

public class Course : IValidatableObject
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public Auditable AuditInformation { get; set; } = new Auditable();

    [Required(ErrorMessage = "A Course Type is required")]
    public string CourseTypeId { get; set; }

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

    public Term Term { get; set; } = new Term();
    public List<WorkUnit> WorkUnits { get; set; } = new();

    // Custom validation logic 
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // If StartDate is greater than EndDate, kick it back
        if (StartDate > EndDate)
        {
            yield return new ValidationResult(
                "Start Date cannot be later than End Date.",
                new[] { nameof(StartDate) });
        }

        //If End Date is less than StartDate, kick it back
        if (EndDate < StartDate)
        {
            yield return new ValidationResult(
                "End Date cannot be earlier than Start Date.",
                new[] {nameof(EndDate) });
        }
    }

}
