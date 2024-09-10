namespace TimetableApp.DataModels.Models;

public class WorkTask
{
    public string Id { get; set; }

    public Auditable AuditInformation { get; set; }

    public string Description { get; set; }

    public string Name { get; set; }

    public string TypeId { get; set; }
}
