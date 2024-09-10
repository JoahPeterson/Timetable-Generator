namespace TimetableApp.DataModels.Models;

public class CourseType
{
    public string Id { get; set; }

    public Auditable AuditInformation { get; set; }

    public string Description { get; set; }

    public string Name { get; set; }
}
