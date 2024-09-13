namespace TimetableApp.DataModels.Models;

public class Course
{
    public string Id { get; set; }

    public Auditable AuditInformation { get; set; }

    public DateTime? DateCreated { get; set; }

    public string? Description { get; set; }

    public DateTime EndDate { get; set; }

    public string Name { get; set; }

    public DateTime StartDate { get; set; }

    public string TermId { get; set; }

    public List<WorkUnit> WorkUnits { get; set; }

}
