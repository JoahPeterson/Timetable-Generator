namespace Timetable.BlazorUI.Data.Models
{
    public class Course
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Guid CreatedById { get; set; }

        public IEnumerable<WorkUnit> WorkUnits { get; set; }

        public Guid TermId { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? LastModified { get; set; }

        public bool IsArchived { get; set; }

    }
}
